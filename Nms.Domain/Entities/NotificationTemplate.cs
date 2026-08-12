using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class NotificationTemplate : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public string Name { get; private set; } = string.Empty;
    public NotificationChannel Channel { get; private set; }
    public string SubjectTemplate { get; private set; } = string.Empty;
    public string BodyTemplate { get; private set; } = string.Empty;
    public string RecipientTarget { get; private set; } = string.Empty;
    public bool IsEnabled { get; private set; }

    private NotificationTemplate() { }

    public NotificationTemplate(
        Guid tenantId,
        string name,
        NotificationChannel channel,
        string subjectTemplate,
        string bodyTemplate,
        string recipientTarget)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        Name = name;
        Channel = channel;
        SubjectTemplate = subjectTemplate;
        BodyTemplate = bodyTemplate;
        RecipientTarget = recipientTarget;
        IsEnabled = true;
    }

    public void Update(
        string name,
        NotificationChannel channel,
        string subjectTemplate,
        string bodyTemplate,
        string recipientTarget)
    {
        Name = name;
        Channel = channel;
        SubjectTemplate = subjectTemplate;
        BodyTemplate = bodyTemplate;
        RecipientTarget = recipientTarget;
    }

    public void Enable() => IsEnabled = true;
    public void Disable() => IsEnabled = false;
}