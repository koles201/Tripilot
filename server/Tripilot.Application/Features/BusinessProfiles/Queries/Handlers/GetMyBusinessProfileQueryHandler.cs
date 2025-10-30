using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Business;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.BusinessProfiles.Queries.Handlers;

public class GetMyBusinessProfileQueryHandler : IRequestHandler<GetMyBusinessProfileQuery, BusinessProfileResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMyBusinessProfileQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BusinessProfileResponse?> Handle(GetMyBusinessProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await _unitOfWork.Repository<BusinessProfile>()
            .GetQueryable()
            .FirstOrDefaultAsync(bp => bp.UserId == request.UserId, cancellationToken);

        return profile == null ? null : _mapper.Map<BusinessProfileResponse>(profile);
    }
}
