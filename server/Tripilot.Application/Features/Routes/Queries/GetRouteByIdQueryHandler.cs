using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Route;

namespace Tripilot.Application.Features.Routes.Queries;

public class GetRouteByIdQueryHandler : IRequestHandler<GetRouteByIdQuery, RouteResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetRouteByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<RouteResponse> Handle(GetRouteByIdQuery request, CancellationToken cancellationToken)
    {
        var route = await _unitOfWork.Repository<Domain.Entities.Route>()
            .GetQueryable()
            .Include(r => r.Creator)
            .Include(r => r.RoutePlaces.OrderBy(rp => rp.Order))
                .ThenInclude(rp => rp.Place)
            .FirstOrDefaultAsync(r => r.Id == request.Id && r.IsActive, cancellationToken);

        if (route == null)
        {
            throw new KeyNotFoundException($"Route with ID {request.Id} not found");
        }

        // Increment view count
        route.ViewCount++;
        _unitOfWork.Repository<Domain.Entities.Route>().Update(route);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = _mapper.Map<RouteResponse>(route);
        return response;
    }
}
