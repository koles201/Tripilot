namespace Tripilot.Application.Common.Interfaces;

/// <summary>
/// Basic email service abstraction for sending notification emails.
/// </summary>
public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
}
