using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Place;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;
using Tripilot.Domain.ValueObjects;

namespace Tripilot.Application.Features.Places.Queries;

/// <summary>
/// Handler for SearchPlacesQuery
/// </summary>
public class SearchPlacesQueryHandler : IRequestHandler<SearchPlacesQuery, PlacesListResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SearchPlacesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PlacesListResult> Handle(SearchPlacesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Place>()
            .GetQueryable()
            .Where(p => p.IsActive);

        // Search by name or description
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            query = query.Where(p => 
                p.Name.ToLower().Contains(searchLower) || 
                p.Description.ToLower().Contains(searchLower));
        }

        // Filter by category
        if (!string.IsNullOrEmpty(request.Category))
        {
            if (Enum.TryParse<PlaceCategory>(request.Category, true, out var category))
            {
                query = query.Where(p => p.Category == category);
            }
        }

        // Get all matching places for distance filtering (if needed)
        var places = await query.ToListAsync(cancellationToken);

        // Filter by distance if coordinates provided
        if (request.Latitude.HasValue && request.Longitude.HasValue && request.RadiusKm.HasValue)
        {
            var searchLocation = Location.Create(request.Latitude.Value, request.Longitude.Value, null, null, null, null);
            
            places = places
                .Where(p => p.Location.DistanceTo(searchLocation) <= request.RadiusKm.Value)
                .ToList();
        }

        // Apply sorting and pagination
        var totalCount = places.Count;
        var paginatedPlaces = places
            .OrderByDescending(p => p.AverageRating)
            .ThenByDescending(p => p.ViewCount)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var placeResponses = _mapper.Map<List<PlaceListResponse>>(paginatedPlaces);

        return new PlacesListResult
        {
            Places = placeResponses,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
