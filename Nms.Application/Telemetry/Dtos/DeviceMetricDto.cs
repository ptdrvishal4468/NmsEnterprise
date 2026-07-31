namespace Nms.Application.Telemetry.Dtos;

public record DeviceMetricDto(
    long Id,
    Guid DeviceId,
    decimal CpuUtilization,
    decimal RamUtilization,
    int LatencyMs,
    DateTime TimestampUtc
);