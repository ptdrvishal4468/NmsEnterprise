using Nms.Domain.Common;

namespace Nms.Domain.Entities;

public class DeviceMetricRaw : BaseEntity<long>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid DeviceId { get; private set; }
    public decimal CpuUtilization { get; private set; }
    public decimal RamUtilization { get; private set; }
    public decimal DiskUtilization { get; private set; }
    public decimal InterfaceUtilization { get; private set; }
    public decimal Temperature { get; private set; }
    public int FanStatus { get; private set; }
    public int PowerSupplyStatus { get; private set; }
    public int LatencyMs { get; private set; }
    public DateTime TimestampUtc { get; private set; }

    private DeviceMetricRaw() { }

    public DeviceMetricRaw(
        Guid tenantId,
        Guid deviceId,
        decimal cpuUtilization,
        decimal ramUtilization,
        decimal diskUtilization,
        decimal interfaceUtilization,
        decimal temperature,
        int fanStatus,
        int powerSupplyStatus,
        int latencyMs)
    {
        TenantId = tenantId;
        DeviceId = deviceId;
        CpuUtilization = Math.Clamp(cpuUtilization, 0m, 100m);
        RamUtilization = Math.Clamp(ramUtilization, 0m, 100m);
        DiskUtilization = Math.Clamp(diskUtilization, 0m, 100m);
        InterfaceUtilization = Math.Clamp(interfaceUtilization, 0m, 100m);
        Temperature = temperature;
        FanStatus = fanStatus;
        PowerSupplyStatus = powerSupplyStatus;
        LatencyMs = latencyMs;
        TimestampUtc = DateTime.UtcNow;
    }
}