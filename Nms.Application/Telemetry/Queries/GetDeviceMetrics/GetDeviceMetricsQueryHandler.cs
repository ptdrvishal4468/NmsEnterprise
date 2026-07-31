using MediatR;
using Nms.Application.Telemetry.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Telemetry.Queries.GetDeviceMetrics;

public class GetDeviceMetricsQueryHandler : IRequestHandler<GetDeviceMetricsQuery, IEnumerable<DeviceMetricDto>>
{
    private readonly IDeviceMetricRepository _repository;

    public GetDeviceMetricsQueryHandler(IDeviceMetricRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DeviceMetricDto>> Handle(GetDeviceMetricsQuery request, CancellationToken cancellationToken)
    {
        var metrics = await _repository.GetMetricsForDeviceAsync(
            request.DeviceId,
            request.FromUtc,
            request.ToUtc,
            cancellationToken);

        return metrics.Select(m => new DeviceMetricDto(
            m.Id,
            m.DeviceId,
            m.CpuUtilization,
            m.RamUtilization,
            m.LatencyMs,
            m.TimestampUtc
        ));
    }
}