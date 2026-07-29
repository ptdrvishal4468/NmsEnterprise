using Nms.Domain.Common;

namespace Nms.Domain.Entities;

public class DeviceMetricRaw : BaseEntity<long>
{
    public Guid DeviceId { get; private set; }
    public decimal CpuUtilization { get; private set; }
    public decimal RamUtilization { get; private set; }
    public int LatencyMs { get; private set; }
    public DateTime TimestampUtc { get; private set; }

    private DeviceMetricRaw() { }

    public DeviceMetricRaw(Guid deviceId, decimal cpuUtilization, decimal ramUtilization, int latencyMs)
    {
        DeviceId = deviceId;
        CpuUtilization = Math.Clamp(cpuUtilization, 0m, 100m);
        RamUtilization = Math.Clamp(ramUtilization, 0m, 100m);
        LatencyMs = latencyMs;
        TimestampUtc = DateTime.UtcNow;
    }
}