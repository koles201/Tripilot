using MediatR;
using Tripilot.Application.DTOs.Business;

namespace Tripilot.Application.Features.BusinessProfiles.Queries;

public class GetMyBusinessProfileQuery : IRequest<BusinessProfileResponse?>
{
    public Guid UserId { get; set; }
}
