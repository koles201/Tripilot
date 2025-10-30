using MediatR;
using Microsoft.Extensions.Logging;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Events;

namespace Tripilot.Application.Features.BusinessProfiles.EventHandlers;

public class BusinessProfileApprovedEventHandler : INotificationHandler<BusinessProfileApprovedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ILogger<BusinessProfileApprovedEventHandler> _logger;

    public BusinessProfileApprovedEventHandler(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ILogger<BusinessProfileApprovedEventHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(BusinessProfileApprovedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Business profile {ProfileId} approved for user {UserId}", 
            notification.BusinessProfileId, notification.UserId);

        var profile = await _unitOfWork.Repository<BusinessProfile>()
            .GetByIdAsync(notification.BusinessProfileId, cancellationToken);

        if (profile != null && !string.IsNullOrEmpty(profile.Email))
        {
            try
            {
                await _emailService.SendAsync(
                    to: profile.Email,
                    subject: "Business Profile Approved! ",
                    htmlBody: $@"
                        <h2>Congratulations!</h2>
                        <p>Your business profile '{profile.BusinessName}' has been approved.</p>
                        <p>Approved at: {notification.ApprovedAt:O}</p>
                        <p>You can now manage your places and access business features.</p>
                    ",
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send approval email for profile {ProfileId}", notification.BusinessProfileId);
            }
        }
    }
}
