using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.Favorites.Commands;

/// <summary>
/// Handler for removing a place from user's favorites
/// </summary>
public class RemoveFavoriteCommandHandler : IRequestHandler<RemoveFavoriteCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveFavoriteCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RemoveFavoriteCommand request, CancellationToken cancellationToken)
    {
        var favorite = await _unitOfWork.Repository<Favorite>()
            .GetQueryable()
            .FirstOrDefaultAsync(
                f => f.UserId == request.UserId && f.PlaceId == request.PlaceId,
                cancellationToken
            );

        if (favorite == null)
        {
            // Return true even if not found (idempotent operation)
            return true;
        }

        _unitOfWork.Repository<Favorite>().Delete(favorite);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
