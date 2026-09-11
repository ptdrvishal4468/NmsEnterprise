using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class AuditLog : BaseEntity<long>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; private set; }
    public string? Username { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string? EntityName { get; private set; }
    public string? EntityId { get; private set; }
    public AuditCategory Category { get; private set; } = AuditCategory.System;
    public AuditStatus Status { get; private set; } = AuditStatus.Success;
    public string? IpAddress { get; private set; }
    public string? Details { get; private set; }
    public string? OldValuesJson { get; private set; }
    public string? NewValuesJson { get; private set; }
    public DateTime TimestampUtc { get; private set; } = DateTime.UtcNow;

    private AuditLog() { }

    public AuditLog(
        Guid tenantId,
        Guid userId,
        string action,
        string? oldValuesJson = null,
        string? newValuesJson = null,
        string? username = null,
        string? entityName = null,
        string? entityId = null,
        AuditCategory category = AuditCategory.System,
        AuditStatus status = AuditStatus.Success,
        string? ipAddress = null,
        string? details = null)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Audit action is required.", nameof(action));

        TenantId = tenantId;
        UserId = userId;
        Action = action;
        OldValuesJson = oldValuesJson;
        NewValuesJson = newValuesJson;
        Username = username;
        EntityName = entityName;
        EntityId = entityId;
        Category = category;
        Status = status;
        IpAddress = ipAddress;
        Details = details;
        TimestampUtc = DateTime.UtcNow;
    }
}