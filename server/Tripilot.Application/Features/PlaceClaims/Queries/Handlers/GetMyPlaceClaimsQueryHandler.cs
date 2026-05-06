using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Common;
using Tripilot.Application.DTOs.PlaceClaim;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.PlaceClaims.Queries.Handlers;

public class GetMyPlaceClaimsQueryHandler : IRequestHandler<GetMyPlaceClaimsQuery, PaginatedResult<PlaceClaimResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMyPlaceClaimsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<PlaceClaimResponse>> Handle(GetMyPlaceClaimsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<PlaceClaim>()
            .GetQueryable()
            .Include(pc => pc.Place)
            .Include(pc => pc.Claimant)
            .Include(pc => pc.BusinessProfile)
            .Include(pc => pc.ReviewedByAdmin)
            .Where(pc => pc.ClaimantUserId == request.UserId);

        if (request.Status.HasValue)
        {
            query = query.Where(pc => pc.Status == request.Status.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(pc => pc.SubmittedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<List<PlaceClaimResponse>>(items);
        return new PaginatedResult<PlaceClaimResponse>(mapped, total, request.Page, request.PageSize);
    }
}
