using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.RouteSharing;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.RouteSharing.Queries;

/// <summary>
/// Handler for GetRouteByShareTokenQuery
/// </summary>
public class GetRouteByShareTokenQueryHandler : IRequestHandler<GetRouteByShareTokenQuery, RouteSocialMetadata>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetRouteByShareTokenQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RouteSocialMetadata> Handle(GetRouteByShareTokenQuery request, CancellationToken cancellationToken)
    {
        var route = await _unitOfWork.Repository<Route>()
            .GetQueryable()
            .Include(r => r.Creator)
            .FirstOrDefaultAsync(r => r.ShareToken == request.ShareToken, cancellationToken);

        if (route == null)
        {
            throw new InvalidOperationException($"Route with share token {request.ShareToken} not found");
        }

        // Increment view count
        route.ViewCount++;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var baseUrl = "https://tripilot.com"; // TODO: Get from configuration
        
        return new RouteSocialMetadata
        {
            Title = route.Name,
            Description = route.Description,
            ImageUrl = route.ImageUrl,
            Url = $"{baseUrl}/routes/shared/{route.ShareToken}",
            CreatorName = route.Creator.FullName,
            AverageRating = route.AverageRating,
            ViewCount = route.ViewCount,
            FavoriteCount = route.FavoriteCount
        };
    }
}
