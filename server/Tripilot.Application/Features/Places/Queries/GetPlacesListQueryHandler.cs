using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Place;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.Places.Queries;

/// <summary>
/// Handler for GetPlacesListQuery
/// </summary>
public class GetPlacesListQueryHandler : IRequestHandler<GetPlacesListQuery, PlacesListResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPlacesListQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PlacesListResult> Handle(GetPlacesListQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Place>()
            .GetQueryable()
            .Where(p => p.IsActive);

        // Apply filters
        if (!string.IsNullOrEmpty(request.Category))
        {
            if (Enum.TryParse<PlaceCategory>(request.Category, true, out var category))
            {
                query = query.Where(p => p.Category == category);
            }
        }

        if (!string.IsNullOrEmpty(request.City))
        {
            query = query.Where(p => p.Location.City != null && p.Location.City.ToLower().Contains(request.City.ToLower()));
        }

        if (!string.IsNullOrEmpty(request.Country))
        {
            query = query.Where(p => p.Location.Country != null && p.Location.Country.ToLower().Contains(request.Country.ToLower()));
        }

        if (request.MinRating.HasValue)
        {
            query = query.Where(p => p.AverageRating >= request.MinRating.Value);
        }

        if (request.PriceLevel.HasValue)
        {
            query = query.Where(p => p.PriceLevel == request.PriceLevel.Value);
        }

        if (request.IsVerified.HasValue)
        {
            query = query.Where(p => p.IsVerified == request.IsVerified.Value);
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var places = await query
            .OrderByDescending(p => p.AverageRating)
            .ThenByDescending(p => p.ViewCount)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var placeResponses = _mapper.Map<List<PlaceListResponse>>(places);

        return new PlacesListResult
        {
            Places = placeResponses,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
