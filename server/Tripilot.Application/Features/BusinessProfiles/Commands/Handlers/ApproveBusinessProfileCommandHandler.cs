using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Business;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.BusinessProfiles.Commands.Handlers;

public class ApproveBusinessProfileCommandHandler : IRequestHandler<ApproveBusinessProfileCommand, BusinessProfileResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ApproveBusinessProfileCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BusinessProfileResponse> Handle(ApproveBusinessProfileCommand request, CancellationToken cancellationToken)
    {
        if (request.AdminRole != UserRole.Admin)
            throw new UnauthorizedAccessException("Only admins can approve profiles");

        var profile = await _unitOfWork.Repository<BusinessProfile>()
            .GetQueryable().FirstOrDefaultAsync(bp => bp.Id == request.Id, cancellationToken);
        if (profile == null) throw new KeyNotFoundException("Business profile not found");
        if (profile.VerificationStatus != VerificationStatus.Submitted)
            throw new InvalidOperationException("Profile must be in Submitted state to approve");

        profile.VerificationStatus = VerificationStatus.Approved;
        profile.VerifiedAt = DateTime.UtcNow;
        profile.RejectionReason = null;

        _unitOfWork.Repository<BusinessProfile>().Update(profile);
        profile.RaiseDomainEvent(new Tripilot.Domain.Events.BusinessProfileApprovedEvent(profile.Id, profile.UserId, profile.VerifiedAt!.Value));
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<BusinessProfileResponse>(profile);
    }
}
