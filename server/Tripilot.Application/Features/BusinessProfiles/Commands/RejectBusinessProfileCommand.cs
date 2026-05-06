using MediatR;
using Tripilot.Application.DTOs.Business;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.BusinessProfiles.Commands;

public class RejectBusinessProfileCommand : IRequest<BusinessProfileResponse>
{
    public Guid Id { get; set; }
    public Guid AdminUserId { get; set; }
    public UserRole AdminRole { get; set; }
    public string? RejectionReason { get; set; }
}
