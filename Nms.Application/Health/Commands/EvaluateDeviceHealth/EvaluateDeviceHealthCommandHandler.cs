using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Health.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Health.Commands.EvaluateDeviceHealth;

public class EvaluateDeviceHealthCommandHandler : IRequestHandler<EvaluateDeviceHealthCommand, DeviceHealthDto>
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IReachabilityHistoryRepository _reachabilityRepository;
    private readonly IDeviceMetricRepository _metricRepository;
    private readonly INetworkInterfaceRepository _interfaceRepository;
    private readonly IDeviceHealthHistoryRepository _healthHistoryRepository;
    private readonly IDeviceHealthCalculator _healthCalculator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public EvaluateDeviceHealthCommandHandler(
        IDeviceRepository deviceRepository,
        IReachabilityHistoryRepository reachabilityRepository,
        IDeviceMetricRepository metricRepository,
        INetworkInterfaceRepository interfaceRepository,
        IDeviceHealthHistoryRepository healthHistoryRepository,
        IDeviceHealthCalculator healthCalculator,
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext)
    {
        _deviceRepository = deviceRepository;
        _reachabilityRepository = reachabilityRepository;
        _metricRepository = metricRepository;
        _interfaceRepository = interfaceRepository;
        _healthHistoryRepository = healthHistoryRepository;
        _healthCalculator = healthCalculator;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<DeviceHealthDto> Handle(EvaluateDeviceHealthCommand request, CancellationToken cancellationToken)
    {
        var device = await _deviceRepository.GetByIdAsync(request.DeviceId, cancellationToken)
            ?? throw new KeyNotFoundException($"Device with ID '{request.DeviceId}' was not found.");

        var latestReachability = await _reachabilityRepository.GetLatestByDeviceIdAsync(request.DeviceId, cancellationToken);

        var fromUtc = DateTime.UtcNow.AddMinutes(-15);
        var toUtc = DateTime.UtcNow;
        var metrics = await _metricRepository.GetMetricsForDeviceAsync(request.DeviceId, fromUtc, toUtc, cancellationToken);
        var latestMetric = metrics.OrderByDescending(m => m.TimestampUtc).FirstOrDefault();

        var interfaces = await _interfaceRepository.GetByDeviceIdAsync(request.DeviceId, cancellationToken);

        var evaluation = _healthCalculator.CalculateHealth(device, latestReachability, latestMetric, interfaces);

        device.UpdateStatus(evaluation.Status);
        _deviceRepository.Update(device);

        var historyLog = new DeviceHealthHistory(
            Guid.NewGuid(),
            _tenantContext.TenantId,
            device.Id,
            evaluation.HealthScore,
            evaluation.Status,
            evaluation.SummaryReason,
            DateTime.UtcNow);

        await _healthHistoryRepository.AddAsync(historyLog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DeviceHealthDto(
            device.Id,
            device.Name,
            evaluation.HealthScore,
            evaluation.Status,
            evaluation.SummaryReason,
            device.LastSeenUtc,
            historyLog.TimestampUtc);
    }
}