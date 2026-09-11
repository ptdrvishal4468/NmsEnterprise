using MediatR;
using Nms.Application.Telemetry.Dtos;

namespace Nms.Application.Telemetry.Queries.GetDeviceMetrics;

public record GetDeviceMetricsQuery(
    Guid DeviceId,
    DateTime FromUtc,
    DateTime ToUtc
) : IRequest<IEnumerable<DeviceMetricDto>>;