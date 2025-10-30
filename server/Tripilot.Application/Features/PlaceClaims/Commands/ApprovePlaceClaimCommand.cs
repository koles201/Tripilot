using MediatR;
using Tripilot.Application.DTOs.PlaceClaim;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.PlaceClaims.Commands;

public class ApprovePlaceClaimCommand : IRequest<PlaceClaimResponse>
{
    public Guid ClaimId { get; set; }
    public Guid AdminUserId { get; set; }
    public UserRole AdminRole { get; set; }
    public string? DecisionReason { get; set; }
}
