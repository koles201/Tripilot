using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Route;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.Routes.Commands;

public class UpdateRouteCommandHandler : IRequestHandler<UpdateRouteCommand, RouteResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateRouteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<RouteResponse> Handle(UpdateRouteCommand request, CancellationToken cancellationToken)
    {
        // Get existing route with related data
        var route = await _unitOfWork.Repository<Domain.Entities.Route>()
            .GetQueryable()
            .Include(r => r.RoutePlaces)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (route == null)
        {
            throw new KeyNotFoundException($"Route with ID {request.Id} not found");
        }

        // Check authorization (only creator can update)
        if (route.CreatorId != request.UserId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this route");
        }

        // Parse enums
        if (!Enum.TryParse<RouteDifficulty>(request.Difficulty, true, out var difficulty))
        {
            throw new ArgumentException($"Invalid difficulty: {request.Difficulty}");
        }

        if (!Enum.TryParse<RoutePrivacy>(request.Privacy, true, out var privacy))
        {
            throw new ArgumentException($"Invalid privacy: {request.Privacy}");
        }

        // Validate places exist
        var placeIds = request.Places.Select(p => p.PlaceId).ToList();
        var places = await _unitOfWork.Repository<Place>()
            .GetQueryable()
            .Where(p => placeIds.Contains(p.Id) && p.IsActive)
            .ToListAsync(cancellationToken);

        if (places.Count != placeIds.Distinct().Count())
        {
            throw new ArgumentException("One or more places not found or inactive");
        }

        // Update route properties
        route.Name = request.Name;
        route.Description = request.Description;
        route.Difficulty = difficulty;
        route.Privacy = privacy;
        route.EstimatedDuration = request.EstimatedDuration;
        route.TotalDistance = request.TotalDistance;
        route.ImageUrl = request.ImageUrl;
        route.Tags = request.Tags;
        route.IsActive = request.IsActive;

        // Remove existing route places
        route.RoutePlaces.Clear();

        // Add updated route places
        foreach (var placeRequest in request.Places)
        {
            route.RoutePlaces.Add(new RoutePlace
            {
                RouteId = route.Id,
                PlaceId = placeRequest.PlaceId,
                Order = placeRequest.Order,
                Notes = placeRequest.Notes,
                EstimatedTimeAtPlace = placeRequest.EstimatedTimeAtPlace
            });
        }

        // Save changes
        _unitOfWork.Repository<Domain.Entities.Route>().Update(route);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Reload with complete data
        var updatedRoute = await _unitOfWork.Repository<Domain.Entities.Route>()
            .GetQueryable()
            .Include(r => r.Creator)
            .Include(r => r.RoutePlaces)
                .ThenInclude(rp => rp.Place)
            .FirstOrDefaultAsync(r => r.Id == route.Id, cancellationToken);

        var response = _mapper.Map<RouteResponse>(updatedRoute);
        return response;
    }
}
