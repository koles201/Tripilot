using MediatR;
using Tripilot.Application.DTOs.Common;
using Tripilot.Application.DTOs.PlaceClaim;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.PlaceClaims.Queries;

public class GetMyPlaceClaimsQuery : IRequest<PaginatedResult<PlaceClaimResponse>>
{
    public Guid UserId { get; set; }
    public ClaimStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
