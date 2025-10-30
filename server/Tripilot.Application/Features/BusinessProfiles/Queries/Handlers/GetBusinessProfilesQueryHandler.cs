using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Business;
using Tripilot.Application.DTOs.Common;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.BusinessProfiles.Queries.Handlers;

public class GetBusinessProfilesQueryHandler : IRequestHandler<GetBusinessProfilesQuery, PaginatedResult<BusinessProfileResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetBusinessProfilesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<BusinessProfileResponse>> Handle(GetBusinessProfilesQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<BusinessProfile>()
            .GetQueryable();

        if (request.VerificationStatus.HasValue)
        {
            query = query.Where(bp => bp.VerificationStatus == request.VerificationStatus.Value);
        }
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.ToLower();
            query = query.Where(bp => bp.BusinessName.ToLower().Contains(term) || (bp.Description != null && bp.Description.ToLower().Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(bp => bp.BusinessName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<List<BusinessProfileResponse>>(items);
        return new PaginatedResult<BusinessProfileResponse>(mapped, total, request.Page, request.PageSize);
    }
}
