using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.PlaceClaim;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.PlaceClaims.Commands.Handlers;

public class SubmitPlaceClaimCommandHandler : IRequestHandler<SubmitPlaceClaimCommand, PlaceClaimResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SubmitPlaceClaimCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PlaceClaimResponse> Handle(SubmitPlaceClaimCommand request, CancellationToken cancellationToken)
    {
        var place = await _unitOfWork.Repository<Place>()
            .GetQueryable()
            .FirstOrDefaultAsync(p => p.Id == request.PlaceId && p.IsActive, cancellationToken);
        
        if (place == null)
            throw new KeyNotFoundException("Place not found or is inactive");

        if (place.OwnerId != null)
            throw new InvalidOperationException("This place is already claimed");

        var businessProfile = await _unitOfWork.Repository<BusinessProfile>()
            .GetQueryable()
            .FirstOrDefaultAsync(bp => bp.UserId == request.UserId, cancellationToken);

        if (businessProfile == null)
            throw new InvalidOperationException("Business profile required to submit a claim");

        if (businessProfile.VerificationStatus != VerificationStatus.Approved)
            throw new InvalidOperationException("Business profile must be verified before claiming places");

        var existingPendingClaim = await _unitOfWork.Repository<PlaceClaim>()
            .GetQueryable()
            .AnyAsync(pc => pc.PlaceId == request.PlaceId && 
                           (pc.Status == ClaimStatus.Pending || pc.Status == ClaimStatus.UnderReview), 
                     cancellationToken);

        if (existingPendingClaim)
            throw new InvalidOperationException("An active claim already exists for this place");

        var claim = new PlaceClaim
        {
            PlaceId = request.PlaceId,
            ClaimantUserId = request.UserId,
            BusinessProfileId = businessProfile.Id,
            Status = ClaimStatus.Pending,
            ClaimReason = request.ClaimReason,
            DocumentUrls = JsonSerializer.Serialize(request.DocumentUrls),
            SubmittedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<PlaceClaim>().AddAsync(claim, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = await _unitOfWork.Repository<PlaceClaim>()
            .GetQueryable()
            .Include(pc => pc.Place)
            .Include(pc => pc.Claimant)
            .Include(pc => pc.BusinessProfile)
            .FirstOrDefaultAsync(pc => pc.Id == claim.Id, cancellationToken);

        return _mapper.Map<PlaceClaimResponse>(result);
    }
}
