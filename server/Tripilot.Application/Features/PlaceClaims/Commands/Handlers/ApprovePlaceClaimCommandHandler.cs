using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.PlaceClaim;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.PlaceClaims.Commands.Handlers;

public class ApprovePlaceClaimCommandHandler : IRequestHandler<ApprovePlaceClaimCommand, PlaceClaimResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ApprovePlaceClaimCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PlaceClaimResponse> Handle(ApprovePlaceClaimCommand request, CancellationToken cancellationToken)
    {
        if (request.AdminRole != UserRole.Admin)
            throw new UnauthorizedAccessException("Only admins can approve claims");

        var claim = await _unitOfWork.Repository<PlaceClaim>()
            .GetByIdAsync(request.ClaimId, cancellationToken);

        if (claim == null)
            throw new KeyNotFoundException("Place claim not found");

        if (claim.Status != ClaimStatus.Pending && claim.Status != ClaimStatus.UnderReview)
            throw new InvalidOperationException("Only pending or under review claims can be approved");

        var place = await _unitOfWork.Repository<Place>()
            .GetByIdAsync(claim.PlaceId, cancellationToken);

        if (place == null)
            throw new KeyNotFoundException("Associated place not found");

        claim.Status = ClaimStatus.Approved;
        claim.ReviewedAt = DateTime.UtcNow;
        claim.ReviewedByAdminId = request.AdminUserId;
        claim.AdminDecisionReason = request.DecisionReason;

        claim.Place.OwnerId = claim.ClaimantUserId;
        claim.Place.IsVerified = true;

        _unitOfWork.Repository<PlaceClaim>().Update(claim);
        _unitOfWork.Repository<Place>().Update(claim.Place);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = await _unitOfWork.Repository<PlaceClaim>()
            .GetQueryable()
            .Include(pc => pc.Place)
            .Include(pc => pc.Claimant)
            .Include(pc => pc.BusinessProfile)
            .Include(pc => pc.ReviewedByAdmin)
            .FirstOrDefaultAsync(pc => pc.Id == claim.Id, cancellationToken);

        return _mapper.Map<PlaceClaimResponse>(result);
    }
}
