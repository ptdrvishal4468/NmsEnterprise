using System.Text;
using System.Text.Json;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Enums;

namespace Nms.Infrastructure.Notifications.Providers;

public class WebhookNotificationProvider : INotificationProvider
{
    private readonly HttpClient _httpClient;

    public NotificationChannel Channel => NotificationChannel.Webhook;

    public WebhookNotificationProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> SendAsync(string recipientTarget, string subject, string body, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            event_type = "ALERT_NOTIFICATION",
            subject,
            message = body,
            timestamp_utc = DateTime.UtcNow
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(recipientTarget, content, cancellationToken);

        return response.IsSuccessStatusCode;
    }
}