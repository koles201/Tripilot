using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Business;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.BusinessProfiles.Commands.Handlers;

public class SubmitVerificationCommandHandler : IRequestHandler<SubmitVerificationCommand, BusinessProfileResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileStorageService _fileStorage;

    public SubmitVerificationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IFileStorageService fileStorage)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileStorage = fileStorage;
    }

    public async Task<BusinessProfileResponse> Handle(SubmitVerificationCommand request, CancellationToken cancellationToken)
    {
        var profile = await _unitOfWork.Repository<BusinessProfile>()
            .GetQueryable()
            .FirstOrDefaultAsync(bp => bp.Id == request.Id, cancellationToken);
        if (profile == null) throw new KeyNotFoundException("Business profile not found");
        if (profile.UserId != request.UserId) throw new UnauthorizedAccessException("Not owner of profile");
        if (profile.VerificationStatus == VerificationStatus.Submitted) throw new InvalidOperationException("Already submitted");
        if (profile.VerificationStatus == VerificationStatus.Approved) throw new InvalidOperationException("Already verified");
    if (request.DocumentUrls.Count == 0) throw new ArgumentException("At least one document required");

    // TODO: Replace when actual upload flows exist: currently assumes DocumentUrls already point to accessible files.
    // Potential future: accept IFormFile list via controller and upload here using _fileStorage.

    profile.VerificationStatus = VerificationStatus.Submitted;
        profile.VerificationSubmittedAt = DateTime.UtcNow;
        profile.RejectionReason = null; // clear previous rejection

        _unitOfWork.Repository<BusinessProfile>().Update(profile);
        profile.RaiseDomainEvent(new Tripilot.Domain.Events.BusinessProfileSubmittedEvent(profile.Id, profile.UserId, profile.VerificationSubmittedAt!.Value));
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<BusinessProfileResponse>(profile);
    }
}
