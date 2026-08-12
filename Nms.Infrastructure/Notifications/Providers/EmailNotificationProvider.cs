using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Enums;
using Nms.Infrastructure.Notifications.Options;

namespace Nms.Infrastructure.Notifications.Providers;

public class EmailNotificationProvider : INotificationProvider
{
    private readonly NotificationOptions _options;

    public NotificationChannel Channel => NotificationChannel.Email;

    public EmailNotificationProvider(IOptions<NotificationOptions> options)
    {
        _options = options.Value;
    }

    public async Task<bool> SendAsync(string recipientTarget, string subject, string body, CancellationToken cancellationToken = default)
    {
        using var client = new SmtpClient(_options.Smtp.Host, _options.Smtp.Port)
        {
            EnableSsl = _options.Smtp.EnableSsl
        };

        if (!string.IsNullOrWhiteSpace(_options.Smtp.Username))
        {
            client.Credentials = new NetworkCredential(_options.Smtp.Username, _options.Smtp.Password);
        }

        using var message = new MailMessage(_options.Smtp.FromEmail, recipientTarget)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };

        await client.SendMailAsync(message, cancellationToken);
        return true;
    }
}