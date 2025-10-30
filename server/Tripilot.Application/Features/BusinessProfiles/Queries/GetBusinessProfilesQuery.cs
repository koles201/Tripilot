using MediatR;
using Tripilot.Application.DTOs.Business;
using Tripilot.Application.DTOs.Common;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.BusinessProfiles.Queries;

public class GetBusinessProfilesQuery : IRequest<PaginatedResult<BusinessProfileResponse>>
{
    public VerificationStatus? VerificationStatus { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
