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

        return metrics.Select(m => new DeviceMetricDto
        {
            Id = m.Id,
            DeviceId = m.DeviceId,
            CpuUtilization = m.CpuUtilization,
            RamUtilization = m.RamUtilization,
            DiskUtilization = m.DiskUtilization,
            InterfaceUtilization = m.InterfaceUtilization,
            Temperature = m.Temperature,
            FanStatus = m.FanStatus,
            PowerSupplyStatus = m.PowerSupplyStatus,
            LatencyMs = m.LatencyMs,
            TimestampUtc = m.TimestampUtc
        });
    }
}