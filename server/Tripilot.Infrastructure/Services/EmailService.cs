using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tripilot.Application.Common.Interfaces;

namespace Tripilot.Infrastructure.Services;

/// <summary>
/// Email service implementation (stub for now - can be replaced with SendGrid, SMTP, etc.)
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Implement actual email sending (SendGrid, SMTP, AWS SES, etc.)
            // For now, just log the email details
            _logger.LogInformation(
                "Email simulation - To: {To}, Subject: {Subject}, Body Length: {BodyLength}",
                to, subject, htmlBody?.Length ?? 0);

            // Simulate async operation
            await Task.Delay(100, cancellationToken);

            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {To} with subject {Subject}", to, subject);
            throw;
        }
    }
}
