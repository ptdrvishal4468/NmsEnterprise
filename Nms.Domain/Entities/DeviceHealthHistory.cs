using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class DeviceHealthHistory : BaseEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid DeviceId { get; private set; }
    public Device Device { get; private set; } = null!;
    public double HealthScore { get; private set; }
    public DeviceStatus Status { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public DateTime TimestampUtc { get; private set; }

    // EF Core private constructor
    private DeviceHealthHistory() { }

    public DeviceHealthHistory(
        Guid id,
        Guid tenantId,
        Guid deviceId,
        double healthScore,
        DeviceStatus status,
        string reason,
        DateTime timestampUtc) : base(id)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.", nameof(tenantId));

        if (deviceId == Guid.Empty)
            throw new ArgumentException("Device ID is required.", nameof(deviceId));

        TenantId = tenantId;
        DeviceId = deviceId;
        HealthScore = Math.Clamp(healthScore, 0.0, 100.0);
        Status = status;
        Reason = reason ?? string.Empty;
        TimestampUtc = timestampUtc;
    }
}