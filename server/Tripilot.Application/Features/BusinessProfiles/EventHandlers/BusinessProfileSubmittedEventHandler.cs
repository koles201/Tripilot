using MediatR;
using Microsoft.Extensions.Logging;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Events;

namespace Tripilot.Application.Features.BusinessProfiles.EventHandlers;

public class BusinessProfileSubmittedEventHandler : INotificationHandler<BusinessProfileSubmittedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ILogger<BusinessProfileSubmittedEventHandler> _logger;

    public BusinessProfileSubmittedEventHandler(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ILogger<BusinessProfileSubmittedEventHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(BusinessProfileSubmittedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Business profile {ProfileId} submitted for verification by user {UserId}", 
            notification.BusinessProfileId, notification.UserId);

        var profile = await _unitOfWork.Repository<BusinessProfile>()
            .GetByIdAsync(notification.BusinessProfileId, cancellationToken);

        if (profile != null && !string.IsNullOrEmpty(profile.Email))
        {
            try
            {
                await _emailService.SendAsync(
                    to: profile.Email,
                    subject: "Business Profile Verification Submitted",
                    htmlBody: $@"
                        <h2>Verification Submitted</h2>
                        <p>Your business profile '{profile.BusinessName}' has been submitted for verification.</p>
                        <p>Submitted at: {notification.SubmittedAt:O}</p>
                        <p>We will review your documents and contact you soon.</p>
                    ",
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send submission email for profile {ProfileId}", notification.BusinessProfileId);
            }
        }
    }
}
