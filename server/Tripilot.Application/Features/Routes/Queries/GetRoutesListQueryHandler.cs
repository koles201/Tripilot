using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Route;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.Routes.Queries;

public class GetRoutesListQueryHandler : IRequestHandler<GetRoutesListQuery, List<RouteListResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetRoutesListQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<RouteListResponse>> Handle(GetRoutesListQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Domain.Entities.Route>()
            .GetQueryable()
            .Include(r => r.Creator)
            .Include(r => r.RoutePlaces)
            .Where(r => r.IsActive);

        // Filter by difficulty
        if (!string.IsNullOrEmpty(request.Difficulty) && Enum.TryParse<RouteDifficulty>(request.Difficulty, true, out var difficulty))
        {
            query = query.Where(r => r.Difficulty == difficulty);
        }

        // Filter by privacy (only show Public routes)
        if (!string.IsNullOrEmpty(request.Privacy) && Enum.TryParse<RoutePrivacy>(request.Privacy, true, out var privacy))
        {
            query = query.Where(r => r.Privacy == privacy);
        }
        else
        {
            // Default: only show public routes
            query = query.Where(r => r.Privacy == RoutePrivacy.Public);
        }

        // Filter by creator
        if (request.CreatorId.HasValue)
        {
            query = query.Where(r => r.CreatorId == request.CreatorId.Value);
        }

        // Filter by featured
        if (request.IsFeatured.HasValue)
        {
            query = query.Where(r => r.IsFeatured == request.IsFeatured.Value);
        }

        // Filter by minimum rating
        if (request.MinRating.HasValue)
        {
            query = query.Where(r => r.AverageRating >= request.MinRating.Value);
        }

        // Search by name or description
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchLower = request.SearchTerm.ToLower();
            query = query.Where(r => r.Name.ToLower().Contains(searchLower) || r.Description.ToLower().Contains(searchLower));
        }

        // Sorting
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.IsDescending ? query.OrderByDescending(r => r.Name) : query.OrderBy(r => r.Name),
            "rating" => request.IsDescending ? query.OrderByDescending(r => r.AverageRating) : query.OrderBy(r => r.AverageRating),
            "views" => request.IsDescending ? query.OrderByDescending(r => r.ViewCount) : query.OrderBy(r => r.ViewCount),
            "favorites" => request.IsDescending ? query.OrderByDescending(r => r.FavoriteCount) : query.OrderBy(r => r.FavoriteCount),
            _ => request.IsDescending ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt)
        };

        // Pagination
        var routes = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var response = _mapper.Map<List<RouteListResponse>>(routes);
        return response;
    }
}
