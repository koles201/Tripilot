using MediatR;
using Tripilot.Application.DTOs.PlaceClaim;

namespace Tripilot.Application.Features.PlaceClaims.Commands;

public class SubmitPlaceClaimCommand : IRequest<PlaceClaimResponse>
{
    public Guid UserId { get; set; }
    public Guid PlaceId { get; set; }
    public string ClaimReason { get; set; } = string.Empty;
    public List<string> DocumentUrls { get; set; } = new();
}
