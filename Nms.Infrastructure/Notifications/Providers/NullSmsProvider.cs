using Microsoft.Extensions.Logging;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Enums;

namespace Nms.Infrastructure.Notifications.Providers;

public class NullSmsProvider : ISmsProvider, INotificationProvider
{
    private readonly ILogger<NullSmsProvider> _logger;

    public NotificationChannel Channel => NotificationChannel.Sms;

    public NullSmsProvider(ILogger<NullSmsProvider> logger)
    {
        _logger = logger;
    }

    public Task<bool> SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[SMS ABSTRACTION] Dispatched SMS to {Phone}: {Message}", phoneNumber, message);
        return Task.FromResult(true);
    }

    public Task<bool> SendAsync(string recipientTarget, string subject, string body, CancellationToken cancellationToken = default)
    {
        string smsContent = $"{subject}: {body}";
        return SendSmsAsync(recipientTarget, smsContent, cancellationToken);
    }
}