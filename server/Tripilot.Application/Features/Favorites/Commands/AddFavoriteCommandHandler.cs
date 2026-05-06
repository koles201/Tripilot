using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Favorite;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.Favorites.Commands;

/// <summary>
/// Handler for adding a place to user's favorites
/// </summary>
public class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommand, FavoriteResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddFavoriteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<FavoriteResponse> Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
    {
        // Check if place exists
        var place = await _unitOfWork.Repository<Place>()
            .GetQueryable()
            .FirstOrDefaultAsync(p => p.Id == request.PlaceId, cancellationToken);

        if (place == null)
        {
            throw new KeyNotFoundException($"Place with ID {request.PlaceId} not found.");
        }

        // Check if already favorited (prevent duplicates)
        var existingFavorite = await _unitOfWork.Repository<Favorite>()
            .GetQueryable()
            .FirstOrDefaultAsync(
                f => f.UserId == request.UserId && f.PlaceId == request.PlaceId,
                cancellationToken
            );

        if (existingFavorite != null)
        {
            // Return existing favorite instead of throwing error (idempotent operation)
            return new FavoriteResponse
            {
                Id = existingFavorite.Id,
                UserId = existingFavorite.UserId,
                PlaceId = existingFavorite.PlaceId,
                PlaceName = place.Name,
                Category = place.Category.ToString(),
                City = place.Location.City,
                Country = place.Location.Country,
                ImageUrl = place.ImageUrl,
                AverageRating = place.AverageRating,
                CreatedAt = existingFavorite.CreatedAt
            };
        }

        // Create new favorite
        var favorite = new Favorite
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            PlaceId = request.PlaceId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<Favorite>().AddAsync(favorite);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new FavoriteResponse
        {
            Id = favorite.Id,
            UserId = favorite.UserId,
            PlaceId = favorite.PlaceId,
            PlaceName = place.Name,
            Category = place.Category.ToString(),
            City = place.Location.City,
            Country = place.Location.Country,
            ImageUrl = place.ImageUrl,
            AverageRating = place.AverageRating,
            CreatedAt = favorite.CreatedAt
        };
    }
}
