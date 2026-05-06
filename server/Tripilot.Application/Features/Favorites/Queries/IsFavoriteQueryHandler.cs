using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Favorite;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.Favorites.Queries;

/// <summary>
/// Handler for checking if a place is favorited
/// </summary>
public class IsFavoriteQueryHandler : IRequestHandler<IsFavoriteQuery, IsFavoriteResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public IsFavoriteQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IsFavoriteResponse> Handle(IsFavoriteQuery request, CancellationToken cancellationToken)
    {
        var favorite = await _unitOfWork.Repository<Favorite>()
            .GetQueryable()
            .FirstOrDefaultAsync(
                f => f.UserId == request.UserId && f.PlaceId == request.PlaceId,
                cancellationToken
            );

        return new IsFavoriteResponse
        {
            IsFavorite = favorite != null,
            FavoriteId = favorite?.Id
        };
    }
}
