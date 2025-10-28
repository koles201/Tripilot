using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Place;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.Places.Queries;

/// <summary>
/// Handler for GetPlaceByIdQuery
/// </summary>
public class GetPlaceByIdQueryHandler : IRequestHandler<GetPlaceByIdQuery, PlaceResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPlaceByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PlaceResponse?> Handle(GetPlaceByIdQuery request, CancellationToken cancellationToken)
    {
        var place = await _unitOfWork.Repository<Place>()
            .GetQueryable()
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.Id == request.PlaceId && p.IsActive, cancellationToken);

        if (place == null)
        {
            return null;
        }

        // Increment view count
        place.ViewCount++;
        _unitOfWork.Repository<Place>().Update(place);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = _mapper.Map<PlaceResponse>(place);
        return response;
    }
}
