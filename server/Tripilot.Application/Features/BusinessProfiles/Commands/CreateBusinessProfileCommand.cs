using MediatR;
using Tripilot.Application.DTOs.Business;

namespace Tripilot.Application.Features.BusinessProfiles.Commands;

public class CreateBusinessProfileCommand : IRequest<BusinessProfileResponse>
{
    public Guid UserId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
}
