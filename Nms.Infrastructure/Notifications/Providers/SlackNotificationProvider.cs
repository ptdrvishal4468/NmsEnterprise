using System.Text;
using System.Text.Json;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Enums;

namespace Nms.Infrastructure.Notifications.Providers;

public class SlackNotificationProvider : INotificationProvider
{
    private readonly HttpClient _httpClient;

    public NotificationChannel Channel => NotificationChannel.Slack;

    public SlackNotificationProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> SendAsync(string recipientTarget, string subject, string body, CancellationToken cancellationToken = default)
    {
        var slackBlockPayload = new
        {
            blocks = new object[]
            {
                new
                {
                    type = "header",
                    text = new { type = "plain_text", text = subject, emoji = true }
                },
                new
                {
                    type = "section",
                    text = new { type = "mrkdwn", text = body }
                }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(slackBlockPayload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(recipientTarget, content, cancellationToken);

        return response.IsSuccessStatusCode;
    }
}