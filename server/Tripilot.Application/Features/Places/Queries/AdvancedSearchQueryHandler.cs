using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Place;

namespace Tripilot.Application.Features.Places.Queries;

/// <summary>
/// Handler for advanced place search with full-text search and comprehensive filtering
/// </summary>
public class AdvancedSearchQueryHandler : IRequestHandler<AdvancedSearchQuery, AdvancedSearchResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AdvancedSearchQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AdvancedSearchResult> Handle(AdvancedSearchQuery request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        // Start with all active places
        var query = _unitOfWork.Repository<Domain.Entities.Place>().GetQueryable()
            .Where(p => p.IsActive);

        // Full-text search across name, description, and tags
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim();
            query = query.Where(p =>
                EF.Functions.Like(p.Name, $"%{searchTerm}%") ||
                EF.Functions.Like(p.Description, $"%{searchTerm}%") ||
                EF.Functions.Like(p.Category.ToString(), $"%{searchTerm}%"));
        }

        // Category filter
        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            // Parse string to enum
            if (Enum.TryParse<Domain.Enums.PlaceCategory>(request.Category, true, out var categoryEnum))
            {
                query = query.Where(p => p.Category == categoryEnum);
            }
        }

        // Location filters
        if (!string.IsNullOrWhiteSpace(request.City))
        {
            query = query.Where(p => p.Location.City == request.City);
        }

        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            query = query.Where(p => p.Location.Country == request.Country);
        }

        // Rating filter
        if (request.MinRating.HasValue)
        {
            query = query.Where(p => p.AverageRating >= request.MinRating.Value);
        }

        if (request.MaxRating.HasValue)
        {
            query = query.Where(p => p.AverageRating <= request.MaxRating.Value);
        }

        // Price level filter
        if (request.MinPriceLevel.HasValue)
        {
            query = query.Where(p => p.PriceLevel >= request.MinPriceLevel.Value);
        }

        if (request.MaxPriceLevel.HasValue)
        {
            query = query.Where(p => p.PriceLevel <= request.MaxPriceLevel.Value);
        }

        // Verification status
        if (request.IsVerified.HasValue)
        {
            query = query.Where(p => p.IsVerified == request.IsVerified.Value);
        }

        // Amenities filter (contains any of the specified amenities)
        if (!string.IsNullOrWhiteSpace(request.Amenities))
        {
            var amenitiesList = request.Amenities.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim().ToLower())
                .ToList();

            if (amenitiesList.Any())
            {
                query = query.Where(p => p.Amenities != null &&
                    amenitiesList.Any(amenity => p.Amenities.ToLower().Contains(amenity)));
            }
        }

        // 24-hour filter
        if (request.Is24Hours.HasValue && request.Is24Hours.Value)
        {
            query = query.Where(p => p.OperatingHours != null && p.OperatingHours.Is24Hours);
        }

        // "Open now" filter - check if current time falls within operating hours
        if (request.IsOpenNow.HasValue && request.IsOpenNow.Value)
        {
            var currentTime = TimeSpan.FromHours(DateTime.UtcNow.Hour) + TimeSpan.FromMinutes(DateTime.UtcNow.Minute);
            var currentDayOfWeek = DateTime.UtcNow.DayOfWeek.ToString().Substring(0, 3); // Mon, Tue, etc.

            query = query.Where(p =>
                p.OperatingHours != null && p.OperatingHours.Is24Hours ||
                (p.OperatingHours != null && p.OperatingHours.OpenTime != null && p.OperatingHours.CloseTime != null &&
                 p.OperatingHours.DaysOfWeek != null && p.OperatingHours.DaysOfWeek.Contains(currentDayOfWeek) &&
                 p.OperatingHours.OpenTime <= currentTime && p.OperatingHours.CloseTime >= currentTime));
        }

        // Geospatial filter (distance-based search)
        if (request.Latitude.HasValue && request.Longitude.HasValue && request.RadiusKm.HasValue)
        {
            var lat = request.Latitude.Value;
            var lon = request.Longitude.Value;
            var radius = request.RadiusKm.Value;

            // Using Haversine formula for distance calculation
            // Note: This is an approximation. For production, consider PostGIS or spatial indexes
            query = query.Where(p =>
                Math.Acos(
                    Math.Sin(lat * Math.PI / 180) * Math.Sin((double)p.Location.Latitude * Math.PI / 180) +
                    Math.Cos(lat * Math.PI / 180) * Math.Cos((double)p.Location.Latitude * Math.PI / 180) *
                    Math.Cos((lon - (double)p.Location.Longitude) * Math.PI / 180)
                ) * 6371 <= radius
            );
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Sorting
        query = ApplySorting(query, request);

        // Pagination
        var places = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Map to response DTOs
        var items = places.Select(p =>
        {
            var response = _mapper.Map<AdvancedSearchResponse>(p);

            // Calculate distance if location-based search
            if (request.Latitude.HasValue && request.Longitude.HasValue)
            {
                response.DistanceKm = CalculateDistance(
                    request.Latitude.Value,
                    request.Longitude.Value,
                    (double)p.Location.Latitude,
                    (double)p.Location.Longitude);
            }

            // Calculate relevance score
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                response.RelevanceScore = CalculateRelevanceScore(p.Name, p.Description, request.SearchTerm);
                response.HighlightedName = HighlightText(p.Name, request.SearchTerm);
                response.HighlightedDescription = HighlightText(p.Description, request.SearchTerm, 150);
            }

            // Parse amenities
            if (!string.IsNullOrWhiteSpace(p.Amenities))
            {
                response.Amenities = p.Amenities.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Trim())
                    .ToList();
            }

            // Check if open now
            if (p.OperatingHours != null && p.OperatingHours.Is24Hours)
            {
                response.IsOpenNow = true;
                response.TodayHours = "Open 24 hours";
            }
            else if (p.OperatingHours != null && p.OperatingHours.OpenTime.HasValue && p.OperatingHours.CloseTime.HasValue)
            {
                var currentTime = TimeSpan.FromHours(DateTime.UtcNow.Hour) + TimeSpan.FromMinutes(DateTime.UtcNow.Minute);
                var currentDayOfWeek = DateTime.UtcNow.DayOfWeek.ToString().Substring(0, 3);

                if (p.OperatingHours.DaysOfWeek != null && p.OperatingHours.DaysOfWeek.Contains(currentDayOfWeek))
                {
                    response.IsOpenNow = p.OperatingHours.OpenTime <= currentTime && p.OperatingHours.CloseTime >= currentTime;
                    response.TodayHours = $"{p.OperatingHours.OpenTime:hh\\:mm} - {p.OperatingHours.CloseTime:hh\\:mm}";
                }
                else
                {
                    response.IsOpenNow = false;
                    response.TodayHours = "Closed today";
                }
            }

            return response;
        }).ToList();

        stopwatch.Stop();

        // Build applied filters summary
        var appliedFilters = new SearchFilters
        {
            SearchTerm = request.SearchTerm,
            Category = request.Category,
            City = request.City,
            Country = request.Country,
            MinRating = request.MinRating,
            MaxRating = request.MaxRating,
            MinPriceLevel = request.MinPriceLevel,
            MaxPriceLevel = request.MaxPriceLevel,
            IsVerified = request.IsVerified,
            Amenities = request.Amenities?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim()).ToList(),
            IsOpenNow = request.IsOpenNow,
            Is24Hours = request.Is24Hours,
            Location = request.Latitude.HasValue && request.Longitude.HasValue && request.RadiusKm.HasValue
                ? new LocationFilter
                {
                    Latitude = request.Latitude.Value,
                    Longitude = request.Longitude.Value,
                    RadiusKm = request.RadiusKm.Value
                }
                : null
        };

        return new AdvancedSearchResult
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            AppliedFilters = appliedFilters,
            ExecutionTimeMs = stopwatch.ElapsedMilliseconds
        };
    }

    private IQueryable<Domain.Entities.Place> ApplySorting(
        IQueryable<Domain.Entities.Place> query,
        AdvancedSearchQuery request)
    {
        var sortBy = request.SortBy?.ToLower() ?? "relevance";
        var sortDirection = request.SortDirection?.ToLower() ?? "desc";

        return sortBy switch
        {
            "rating" => sortDirection == "asc"
                ? query.OrderBy(p => p.AverageRating)
                : query.OrderByDescending(p => p.AverageRating),

            "popularity" => sortDirection == "asc"
                ? query.OrderBy(p => p.ViewCount).ThenBy(p => p.ReviewCount)
                : query.OrderByDescending(p => p.ViewCount).ThenByDescending(p => p.ReviewCount),

            "newest" => sortDirection == "asc"
                ? query.OrderBy(p => p.CreatedAt)
                : query.OrderByDescending(p => p.CreatedAt),

            "name" => sortDirection == "asc"
                ? query.OrderBy(p => p.Name)
                : query.OrderByDescending(p => p.Name),

            "distance" when request.Latitude.HasValue && request.Longitude.HasValue => 
                // Distance sorting (approximate - in production use spatial indexes)
                query.OrderBy(p => 
                    Math.Pow((double)p.Location.Latitude - request.Latitude.Value, 2) +
                    Math.Pow((double)p.Location.Longitude - request.Longitude.Value, 2)),

            // Default: relevance (combination of rating, review count, and view count)
            _ => query.OrderByDescending(p => (double)p.AverageRating * 0.4 + p.ReviewCount * 0.3 + p.ViewCount * 0.0001)
                .ThenByDescending(p => p.IsVerified)
        };
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // Haversine formula
        var r = 6371; // Earth's radius in kilometers
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return r * c;
    }

    private double CalculateRelevanceScore(string name, string description, string searchTerm)
    {
        var term = searchTerm.ToLower();
        var nameLower = name.ToLower();
        var descLower = description.ToLower();

        double score = 0;

        // Exact match in name = highest score
        if (nameLower == term) score += 1.0;
        else if (nameLower.Contains(term)) score += 0.7;
        else if (nameLower.Contains(term.Split(' ')[0])) score += 0.5;

        // Match in description
        if (descLower.Contains(term)) score += 0.3;

        return Math.Min(score, 1.0);
    }

    private string HighlightText(string text, string searchTerm, int maxLength = 200)
    {
        if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(searchTerm))
            return text;

        var term = searchTerm.Trim();
        var index = text.IndexOf(term, StringComparison.OrdinalIgnoreCase);

        if (index == -1)
            return text.Length > maxLength ? text.Substring(0, maxLength) + "..." : text;

        // Extract context around the match
        var start = Math.Max(0, index - 50);
        var end = Math.Min(text.Length, index + term.Length + 50);
        var excerpt = text.Substring(start, end - start);

        if (start > 0) excerpt = "..." + excerpt;
        if (end < text.Length) excerpt += "...";

        // Add highlight markers (client will replace with HTML)
        return excerpt.Replace(term, $"<mark>{term}</mark>", StringComparison.OrdinalIgnoreCase);
    }
}
