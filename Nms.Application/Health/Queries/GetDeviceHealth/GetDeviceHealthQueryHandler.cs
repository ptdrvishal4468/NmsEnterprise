using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Health.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Health.Queries.GetDeviceHealth;

public class GetDeviceHealthQueryHandler : IRequestHandler<GetDeviceHealthQuery, DeviceHealthDto>
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IReachabilityHistoryRepository _reachabilityRepository;
    private readonly IDeviceMetricRepository _metricRepository;
    private readonly INetworkInterfaceRepository _interfaceRepository;
    private readonly IDeviceHealthCalculator _healthCalculator;

    public GetDeviceHealthQueryHandler(
        IDeviceRepository deviceRepository,
        IReachabilityHistoryRepository reachabilityRepository,
        IDeviceMetricRepository metricRepository,
        INetworkInterfaceRepository interfaceRepository,
        IDeviceHealthCalculator healthCalculator)
    {
        _deviceRepository = deviceRepository;
        _reachabilityRepository = reachabilityRepository;
        _metricRepository = metricRepository;
        _interfaceRepository = interfaceRepository;
        _healthCalculator = healthCalculator;
    }

    public async Task<DeviceHealthDto> Handle(GetDeviceHealthQuery request, CancellationToken cancellationToken)
    {
        var device = await _deviceRepository.GetByIdAsync(request.DeviceId, cancellationToken)
            ?? throw new KeyNotFoundException($"Device with ID '{request.DeviceId}' was not found.");

        var latestReachability = await _reachabilityRepository.GetLatestByDeviceIdAsync(request.DeviceId, cancellationToken);

        var fromUtc = DateTime.UtcNow.AddMinutes(-15);
        var toUtc = DateTime.UtcNow;
        var metrics = await _metricRepository.GetMetricsForDeviceAsync(request.DeviceId, fromUtc, toUtc, cancellationToken);
        var latestMetric = metrics.OrderByDescending(m => m.TimestampUtc).FirstOrDefault();

        var interfaces = await _interfaceRepository.GetByDeviceIdAsync(request.DeviceId, cancellationToken);

        var result = _healthCalculator.CalculateHealth(device, latestReachability, latestMetric, interfaces);

        return new DeviceHealthDto(
            device.Id,
            device.Name,
            result.HealthScore,
            result.Status,
            result.SummaryReason,
            device.LastSeenUtc,
            DateTime.UtcNow);
    }
}