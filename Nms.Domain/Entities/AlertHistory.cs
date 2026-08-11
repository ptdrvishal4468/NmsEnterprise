using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class AlertHistory : BaseEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid AlertId { get; private set; }
    public AlertState OldState { get; private set; }
    public AlertState NewState { get; private set; }
    public string ChangedBy { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;
    public DateTime TimestampUtc { get; private set; }

    public Alert? Alert { get; private set; }

    private AlertHistory() { }

    public AlertHistory(
        Guid tenantId,
        Guid alertId,
        AlertState oldState,
        AlertState newState,
        string changedBy,
        string note)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        AlertId = alertId;
        OldState = oldState;
        NewState = newState;
        ChangedBy = changedBy;
        Note = note;
        TimestampUtc = DateTime.UtcNow;
    }
}