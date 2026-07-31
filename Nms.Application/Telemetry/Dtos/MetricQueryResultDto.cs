namespace Nms.Application.Telemetry.Dtos;

public record MetricQueryResultDto(
    Guid DeviceId,
    IEnumerable<DeviceMetricDto> Metrics,
    int TotalRecords
);