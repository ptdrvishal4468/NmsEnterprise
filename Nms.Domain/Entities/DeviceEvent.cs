using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class DeviceEvent : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid? DeviceId { get; private set; }
    public EventCategory Category { get; private set; }
    public EventSeverity Severity { get; private set; }
    public string Source { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string? CorrelationId { get; private set; }
    public Guid? ParentEventId { get; private set; }
    public string? MetadataJson { get; private set; }

    // Navigation property
    public Device? Device { get; private set; }

    // EF Core constructor
    protected DeviceEvent() { }

    public DeviceEvent(
        Guid id,
        Guid tenantId,
        EventCategory category,
        EventSeverity severity,
        string source,
        string message,
        Guid? deviceId = null,
        string? correlationId = null,
        Guid? parentEventId = null,
        string? metadataJson = null) : base(id)
    {
        TenantId = tenantId;
        Category = category;
        Severity = severity;
        Source = string.IsNullOrWhiteSpace(source) ? throw new ArgumentNullException(nameof(source)) : source;
        Message = string.IsNullOrWhiteSpace(message) ? throw new ArgumentNullException(nameof(message)) : message;
        DeviceId = deviceId;
        CorrelationId = correlationId;
        ParentEventId = parentEventId;
        MetadataJson = metadataJson;
    }
}