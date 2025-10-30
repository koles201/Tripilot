using Tripilot.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Tripilot.Infrastructure.Services;

/// <summary>
/// Simple email service that logs emails to console (development stub).
/// </summary>
public class ConsoleEmailService : IEmailService
{
    private readonly ILogger<ConsoleEmailService> _logger;
    public ConsoleEmailService(ILogger<ConsoleEmailService> logger) => _logger = logger;

    public Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[EmailStub] To: {To}\nSubject: {Subject}\nBody: {Body}", to, subject, htmlBody);
        return Task.CompletedTask;
    }
}
