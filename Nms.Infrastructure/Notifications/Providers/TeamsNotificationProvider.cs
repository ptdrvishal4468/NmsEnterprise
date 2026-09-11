using System.Text;
using System.Text.Json;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Enums;

namespace Nms.Infrastructure.Notifications.Providers;

public class TeamsNotificationProvider : INotificationProvider
{
    private readonly HttpClient _httpClient;

    public NotificationChannel Channel => NotificationChannel.MicrosoftTeams;

    public TeamsNotificationProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> SendAsync(string recipientTarget, string subject, string body, CancellationToken cancellationToken = default)
    {
        var adaptiveCardPayload = new
        {
            type = "message",
            attachments = new[]
            {
                new
                {
                    contentType = "application/vnd.microsoft.card.adaptive",
                    content = new
                    {
                        type = "AdaptiveCard",
                        version = "1.2",
                        body = new object[]
                        {
                            new { type = "TextBlock", text = subject, weight = "Bolder", size = "Medium" },
                            new { type = "TextBlock", text = body, wrap = true }
                        }
                    }
                }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(adaptiveCardPayload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(recipientTarget, content, cancellationToken);

        return response.IsSuccessStatusCode;
    }
}