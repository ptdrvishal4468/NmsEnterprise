using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class NotificationLog : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid? AlertId { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public string RecipientTarget { get; private set; } = string.Empty;
    public string Subject { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public NotificationStatus Status { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime SentAtUtc { get; private set; }

    public Alert? Alert { get; private set; }

    private NotificationLog() { }

    public NotificationLog(
        Guid tenantId,
        Guid? alertId,
        NotificationChannel channel,
        string recipientTarget,
        string subject,
        string message)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        AlertId = alertId;
        Channel = channel;
        RecipientTarget = recipientTarget;
        Subject = subject;
        Message = message;
        Status = NotificationStatus.Pending;
        SentAtUtc = DateTime.UtcNow;
    }

    public void MarkAsSent()
    {
        Status = NotificationStatus.Sent;
        SentAtUtc = DateTime.UtcNow;
    }

    public void MarkAsFailed(string errorMessage)
    {
        Status = NotificationStatus.Failed;
        ErrorMessage = errorMessage;
        SentAtUtc = DateTime.UtcNow;
    }
}