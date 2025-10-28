using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Route;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.Routes.Commands;

/// <summary>
/// Handler for CreateRouteCommand
/// </summary>
public class CreateRouteCommandHandler : IRequestHandler<CreateRouteCommand, RouteResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateRouteCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<RouteResponse> Handle(CreateRouteCommand request, CancellationToken cancellationToken)
    {
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

        // Create route entity
        var route = new Domain.Entities.Route
        {
            Name = request.Name,
            Description = request.Description,
            Difficulty = difficulty,
            Privacy = privacy,
            EstimatedDuration = request.EstimatedDuration,
            TotalDistance = request.TotalDistance,
            ImageUrl = request.ImageUrl,
            Tags = request.Tags,
            CreatorId = request.UserId,
            IsActive = true,
            IsFeatured = false,
            AverageRating = 0,
            ReviewCount = 0,
            ViewCount = 0,
            FavoriteCount = 0
        };

        // Add route places
        foreach (var placeRequest in request.Places)
        {
            route.RoutePlaces.Add(new RoutePlace
            {
                PlaceId = placeRequest.PlaceId,
                Order = placeRequest.Order,
                Notes = placeRequest.Notes,
                EstimatedTimeAtPlace = placeRequest.EstimatedTimeAtPlace
            });
        }

        // Save to database
        await _unitOfWork.Repository<Domain.Entities.Route>().AddAsync(route, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Load route with related data for response
        var createdRoute = await _unitOfWork.Repository<Domain.Entities.Route>()
            .GetQueryable()
            .Include(r => r.Creator)
            .Include(r => r.RoutePlaces)
                .ThenInclude(rp => rp.Place)
            .FirstOrDefaultAsync(r => r.Id == route.Id, cancellationToken);

        var response = _mapper.Map<RouteResponse>(createdRoute);
        return response;
    }
}
