using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;

namespace Nms.Application.Notifications.Services;

public class NotificationTemplateEngine : INotificationTemplateEngine
{
    public string Render(string templateText, Alert alert, Device? device = null)
    {
        if (string.IsNullOrWhiteSpace(templateText)) return string.Empty;

        return templateText
            .Replace("{AlertSeverity}", alert.Severity.ToString())
            .Replace("{MetricType}", alert.MetricType.ToString())
            .Replace("{MetricValue}", alert.MetricValue.ToString("F2"))
            .Replace("{ThresholdValue}", alert.ThresholdValue.ToString("F2"))
            .Replace("{Message}", alert.Message)
            .Replace("{State}", alert.State.ToString())
            .Replace("{TriggeredAtUtc}", alert.TriggeredAtUtc.ToString("yyyy-MM-dd HH:mm:ss UTC"))
            .Replace("{DeviceName}", device?.Name ?? alert.DeviceId.ToString())
            .Replace("{DeviceIp}", device?.IpAddress ?? "N/A");
    }
}