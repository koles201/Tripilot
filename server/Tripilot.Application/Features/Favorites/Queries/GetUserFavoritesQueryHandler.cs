using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Favorite;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.Favorites.Queries;

/// <summary>
/// Handler for getting user's favorites
/// </summary>
public class GetUserFavoritesQueryHandler : IRequestHandler<GetUserFavoritesQuery, FavoriteListResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserFavoritesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<FavoriteListResult> Handle(GetUserFavoritesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Favorite>()
            .GetQueryable()
            .Include(f => f.Place)
            .Where(f => f.UserId == request.UserId);

        // Filter by category
        if (!string.IsNullOrEmpty(request.Category) && Enum.TryParse<PlaceCategory>(request.Category, out var category))
        {
            query = query.Where(f => f.Place.Category == category);
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Sort
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.SortDirection?.ToLower() == "asc"
                ? query.OrderBy(f => f.Place.Name)
                : query.OrderByDescending(f => f.Place.Name),
            "rating" => request.SortDirection?.ToLower() == "asc"
                ? query.OrderBy(f => f.Place.AverageRating)
                : query.OrderByDescending(f => f.Place.AverageRating),
            _ => request.SortDirection?.ToLower() == "asc"
                ? query.OrderBy(f => f.CreatedAt)
                : query.OrderByDescending(f => f.CreatedAt)
        };

        // Pagination
        var favorites = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(f => new FavoriteResponse
            {
                Id = f.Id,
                UserId = f.UserId,
                PlaceId = f.PlaceId,
                PlaceName = f.Place.Name,
                Category = f.Place.Category.ToString(),
                City = f.Place.Location.City,
                Country = f.Place.Location.Country,
                ImageUrl = f.Place.ImageUrl,
                AverageRating = f.Place.AverageRating,
                CreatedAt = f.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new FavoriteListResult
        {
            Favorites = favorites,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };
    }
}
