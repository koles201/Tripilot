using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Business;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.BusinessProfiles.Commands.Handlers;

public class RejectBusinessProfileCommandHandler : IRequestHandler<RejectBusinessProfileCommand, BusinessProfileResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RejectBusinessProfileCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BusinessProfileResponse> Handle(RejectBusinessProfileCommand request, CancellationToken cancellationToken)
    {
        if (request.AdminRole != UserRole.Admin)
            throw new UnauthorizedAccessException("Only admins can reject profiles");

        var profile = await _unitOfWork.Repository<BusinessProfile>()
            .GetQueryable().FirstOrDefaultAsync(bp => bp.Id == request.Id, cancellationToken);
        if (profile == null) throw new KeyNotFoundException("Business profile not found");
        if (profile.VerificationStatus != VerificationStatus.Submitted)
            throw new InvalidOperationException("Profile must be in Submitted state to reject");

        profile.VerificationStatus = VerificationStatus.Rejected;
        profile.VerifiedAt = null;
        profile.RejectionReason = request.RejectionReason;

        _unitOfWork.Repository<BusinessProfile>().Update(profile);
        profile.RaiseDomainEvent(new Tripilot.Domain.Events.BusinessProfileRejectedEvent(profile.Id, profile.UserId, profile.RejectionReason, DateTime.UtcNow));
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<BusinessProfileResponse>(profile);
    }
}
