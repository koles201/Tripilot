using MediatR;
using Tripilot.Application.DTOs.Business;

namespace Tripilot.Application.Features.BusinessProfiles.Commands;

public class SubmitVerificationCommand : IRequest<BusinessProfileResponse>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<string> DocumentUrls { get; set; } = new();
}
