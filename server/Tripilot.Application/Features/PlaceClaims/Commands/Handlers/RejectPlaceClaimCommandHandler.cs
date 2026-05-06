using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.PlaceClaim;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.PlaceClaims.Commands.Handlers;

public class RejectPlaceClaimCommandHandler : IRequestHandler<RejectPlaceClaimCommand, PlaceClaimResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RejectPlaceClaimCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PlaceClaimResponse> Handle(RejectPlaceClaimCommand request, CancellationToken cancellationToken)
    {
        if (request.AdminRole != UserRole.Admin)
            throw new UnauthorizedAccessException("Only admins can reject claims");

        var claim = await _unitOfWork.Repository<PlaceClaim>()
            .GetByIdAsync(request.ClaimId, cancellationToken);

        if (claim == null)
            throw new KeyNotFoundException("Place claim not found");

        if (claim.Status != ClaimStatus.Pending && claim.Status != ClaimStatus.UnderReview)
            throw new InvalidOperationException("Only pending or under review claims can be rejected");

        claim.Status = ClaimStatus.Rejected;
        claim.ReviewedAt = DateTime.UtcNow;
        claim.ReviewedByAdminId = request.AdminUserId;
        claim.AdminDecisionReason = request.RejectionReason;

        _unitOfWork.Repository<PlaceClaim>().Update(claim);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PlaceClaimResponse>(claim);
    }
}
