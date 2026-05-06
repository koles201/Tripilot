using MediatR;
using Microsoft.Extensions.Logging;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Events;

namespace Tripilot.Application.Features.BusinessProfiles.EventHandlers;

public class BusinessProfileRejectedEventHandler : INotificationHandler<BusinessProfileRejectedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ILogger<BusinessProfileRejectedEventHandler> _logger;

    public BusinessProfileRejectedEventHandler(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ILogger<BusinessProfileRejectedEventHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(BusinessProfileRejectedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Business profile {ProfileId} rejected for user {UserId}", 
            notification.BusinessProfileId, notification.UserId);

        var profile = await _unitOfWork.Repository<BusinessProfile>()
            .GetByIdAsync(notification.BusinessProfileId, cancellationToken);

        if (profile != null && !string.IsNullOrEmpty(profile.Email))
        {
            try
            {
                await _emailService.SendAsync(
                    to: profile.Email,
                    subject: "Business Profile Verification - Action Required",
                    htmlBody: $@"
                        <h2>Verification Update</h2>
                        <p>Your business profile '{profile.BusinessName}' verification was not approved.</p>
                        <p><strong>Reason:</strong> {notification.Reason ?? "Not specified"}</p>
                        <p>Please update your profile and resubmit with the required documentation.</p>
                    ",
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send rejection email for profile {ProfileId}", notification.BusinessProfileId);
            }
        }
    }
}
