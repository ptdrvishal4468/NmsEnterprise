using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class SyslogMessage : AuditableEntity<Guid>, IMustHaveTenant
{
    public SyslogMessage() : base(Guid.NewGuid()) { }
    public SyslogMessage(Guid id) : base(id) { }

    public Guid TenantId { get; set; }
    public Guid? DeviceId { get; set; }
    public SyslogFacility Facility { get; set; }
    public SyslogSeverity Severity { get; set; }
    public string FacilityName { get; set; } = string.Empty;
    public string SeverityName { get; set; } = string.Empty;
    public DateTime TimestampUtc { get; set; }
    public string? Hostname { get; set; }
    public string? AppTag { get; set; }
    public string? ProcessId { get; set; }
    public string? MessageId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? RawMessage { get; set; }
    public string SourceIpAddress { get; set; } = string.Empty;
    public bool IsMalformed { get; set; }

    // Navigation property
    public virtual Device? Device { get; set; }
}