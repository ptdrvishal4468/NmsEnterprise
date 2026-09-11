using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Dashboard.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Dashboard.Queries.GetExecutiveDashboard;

public class GetExecutiveDashboardQueryHandler : IRequestHandler<GetExecutiveDashboardQuery, ExecutiveDashboardDto>
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IAlertRepository _alertRepository;
    private readonly IDeviceHealthHistoryRepository _healthHistoryRepository;
    private readonly ITopologyLinkRepository _topologyLinkRepository;
    private readonly IEventRepository _eventRepository;
    private readonly ITenantContext _tenantContext;
    private readonly ICacheService? _cacheService;

    public GetExecutiveDashboardQueryHandler(
        IDeviceRepository deviceRepository,
        IAlertRepository alertRepository,
        IDeviceHealthHistoryRepository healthHistoryRepository,
        ITopologyLinkRepository topologyLinkRepository,
        IEventRepository eventRepository,
        ITenantContext tenantContext,
        ICacheService? cacheService = null)
    {
        _deviceRepository = deviceRepository;
        _alertRepository = alertRepository;
        _healthHistoryRepository = healthHistoryRepository;
        _topologyLinkRepository = topologyLinkRepository;
        _eventRepository = eventRepository;
        _tenantContext = tenantContext;
        _cacheService = cacheService;
    }

    public async Task<ExecutiveDashboardDto> Handle(GetExecutiveDashboardQuery request, CancellationToken cancellationToken)
    {
        const string cacheKey = "dashboard:executive";
        if (_cacheService != null)
        {
            var cached = await _cacheService.GetAsync<ExecutiveDashboardDto>(cacheKey, cancellationToken);
            if (cached != null)
            {
                return cached;
            }
        }

        // 1. Devices summary using verified DeviceStatus enum
        var devices = await _deviceRepository.GetAllAsync(cancellationToken);
        int totalDevices = devices.Count;
        int onlineDevices = devices.Count(d => d.Status == DeviceStatus.Online);
        int offlineDevices = devices.Count(d => d.Status == DeviceStatus.Offline);
        int degradedDevices = devices.Count(d => d.Status == DeviceStatus.Degraded);
        int unreachableDevices = devices.Count(d => d.Status == DeviceStatus.Unreachable);
        int unknownDevices = devices.Count(d => d.Status == DeviceStatus.Unknown);

        // 2. Alerts summary (Active alerts)
        var activeAlerts = await _alertRepository.FindAsync(a => a.State == AlertState.Active, cancellationToken);
        int totalActiveAlerts = activeAlerts.Count;
        int criticalAlerts = activeAlerts.Count(a => a.Severity == AlertSeverity.Critical);
        int warningAlerts = activeAlerts.Count(a => a.Severity == AlertSeverity.Warning);

        // 3. Health summary (Average health score from recent health histories or fallback calculation)
        var recentCutoff = DateTime.UtcNow.AddHours(-24);
        var recentHealthHistories = await _healthHistoryRepository.FindAsync(
            h => h.TimestampUtc >= recentCutoff,
            cancellationToken);

        double averageHealthScore = 100.0;
        if (recentHealthHistories.Count > 0)
        {
            var latestScores = recentHealthHistories
                .GroupBy(h => h.DeviceId)
                .Select(g => g.OrderByDescending(h => h.TimestampUtc).First().HealthScore)
                .ToList();

            if (latestScores.Count > 0)
            {
                averageHealthScore = Math.Round(latestScores.Average(), 2);
            }
        }
        else if (totalDevices > 0 && (degradedDevices > 0 || unreachableDevices > 0 || offlineDevices > 0))
        {
            double scoreSum = devices.Sum(d => d.Status switch
            {
                DeviceStatus.Online => 100.0,
                DeviceStatus.Degraded => 60.0,
                DeviceStatus.Unreachable => 20.0,
                DeviceStatus.Offline => 0.0,
                _ => 100.0
            });
            averageHealthScore = Math.Round(scoreSum / totalDevices, 2);
        }

        // 4. Topology summary (Active links)
        var activeLinks = await _topologyLinkRepository.GetLinksAsync(
            status: TopologyLinkStatus.Active,
            cancellationToken: cancellationToken);
        int activeTopologyLinks = activeLinks.Count;

        // 5. Recent events summary (Past 24 hours)
        var tenantId = _tenantContext.TenantId;
        var (_, recentEventsCount) = await _eventRepository.GetEventsPagedAsync(
            tenantId,
            pageNumber: 1,
            pageSize: 1,
            fromUtc: recentCutoff,
            cancellationToken: cancellationToken);

        var result = new ExecutiveDashboardDto
        {
            TotalDevices = totalDevices,
            OnlineDevices = onlineDevices,
            OfflineDevices = offlineDevices,
            DegradedDevices = degradedDevices,
            UnreachableDevices = unreachableDevices,
            UnknownDevices = unknownDevices,
            TotalActiveAlerts = totalActiveAlerts,
            CriticalAlerts = criticalAlerts,
            WarningAlerts = warningAlerts,
            AverageHealthScore = averageHealthScore,
            ActiveTopologyLinks = activeTopologyLinks,
            RecentEventsCount = recentEventsCount,
            GeneratedAtUtc = DateTime.UtcNow
        };

        if (_cacheService != null)
        {
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromSeconds(30), cancellationToken);
        }

        return result;
    }
}