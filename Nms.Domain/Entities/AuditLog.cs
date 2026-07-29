using Nms.Domain.Common;

namespace Nms.Domain.Entities;

public class AuditLog : BaseEntity<long>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string? OldValuesJson { get; private set; }
    public string? NewValuesJson { get; private set; }
    public DateTime TimestampUtc { get; private set; } = DateTime.UtcNow;

    private AuditLog() { }

    public AuditLog(
        Guid tenantId,
        Guid userId,
        string action,
        string? oldValuesJson = null,
        string? newValuesJson = null)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Audit action is required.", nameof(action));

        TenantId = tenantId;
        UserId = userId;
        Action = action;
        OldValuesJson = oldValuesJson;
        NewValuesJson = newValuesJson;
        TimestampUtc = DateTime.UtcNow;
    }
}