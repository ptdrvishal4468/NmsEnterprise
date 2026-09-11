namespace Nms.Application.Telemetry.Dtos;

public record DeviceMetricDto
{
    public long Id { get; init; }
    public Guid DeviceId { get; init; }
    public decimal CpuUtilization { get; init; }
    public decimal RamUtilization { get; init; }
    public decimal DiskUtilization { get; init; }
    public decimal InterfaceUtilization { get; init; }
    public decimal Temperature { get; init; }
    public int FanStatus { get; init; }
    public int PowerSupplyStatus { get; init; }
    public int LatencyMs { get; init; }
    public DateTime TimestampUtc { get; init; }
}