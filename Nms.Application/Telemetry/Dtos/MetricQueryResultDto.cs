namespace Nms.Application.Telemetry.Dtos;

public record MetricQueryResultDto(
    Guid DeviceId,
    DateTime FromUtc,
    DateTime ToUtc,
    int TotalRecords,
    IEnumerable<DeviceMetricDto> Metrics
);