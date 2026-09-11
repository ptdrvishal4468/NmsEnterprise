using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Dashboard.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Dashboard.Queries.GetTenantDashboard;

public class GetTenantDashboardQueryHandler : IRequestHandler<GetTenantDashboardQuery, TenantDashboardDto>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IAlertRepository _alertRepository;
    private readonly IDeviceHealthHistoryRepository _healthHistoryRepository;
    private readonly ITopologyLinkRepository _topologyLinkRepository;
    private readonly ITenantContext _tenantContext;
    private readonly ICacheService? _cacheService;

    public GetTenantDashboardQueryHandler(
        ITenantRepository tenantRepository,
        IDeviceRepository deviceRepository,
        IAlertRepository alertRepository,
        IDeviceHealthHistoryRepository healthHistoryRepository,
        ITopologyLinkRepository topologyLinkRepository,
        ITenantContext tenantContext,
        ICacheService? cacheService = null)
    {
        _tenantRepository = tenantRepository;
        _deviceRepository = deviceRepository;
        _alertRepository = alertRepository;
        _healthHistoryRepository = healthHistoryRepository;
        _topologyLinkRepository = topologyLinkRepository;
        _tenantContext = tenantContext;
        _cacheService = cacheService;
    }

    public async Task<TenantDashboardDto> Handle(GetTenantDashboardQuery request, CancellationToken cancellationToken)
    {
        var targetTenantId = request.TenantId ?? _tenantContext.TenantId;

        if (targetTenantId == Guid.Empty)
        {
            throw new InvalidOperationException("Tenant context could not be resolved.");
        }

        var cacheKey = $"dashboard:tenant:{targetTenantId}";
        if (_cacheService != null)
        {
            var cached = await _cacheService.GetAsync<TenantDashboardDto>(cacheKey, cancellationToken);
            if (cached != null)
            {
                return cached;
            }
        }

        var tenant = await _tenantRepository.GetByIdAsync(targetTenantId, cancellationToken)
            ?? throw new KeyNotFoundException($"Tenant with ID '{targetTenantId}' was not found.");

        // 1. Devices summary
        var devices = await _deviceRepository.GetAllAsync(cancellationToken);
        int totalDevices = devices.Count;
        int onlineDevices = devices.Count(d => d.Status == DeviceStatus.Online);
        int offlineDevices = devices.Count(d => d.Status == DeviceStatus.Offline);
        int degradedDevices = devices.Count(d => d.Status == DeviceStatus.Degraded);
        int unreachableDevices = devices.Count(d => d.Status == DeviceStatus.Unreachable);

        // 2. Active alerts count
        var activeAlerts = await _alertRepository.FindAsync(
            a => a.State == AlertState.Active,
            cancellationToken);
        int activeAlertsCount = activeAlerts.Count;

        // 3. Average health score calculation
        var recentCutoff = DateTime.UtcNow.AddHours(-24);
        var recentHistories = await _healthHistoryRepository.FindAsync(
            h => h.TimestampUtc >= recentCutoff,
            cancellationToken);

        double averageHealthScore = 100.0;
        if (recentHistories.Count > 0)
        {
            var latestScores = recentHistories
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

        // 4. Topology links count
        var links = await _topologyLinkRepository.GetLinksAsync(cancellationToken: cancellationToken);
        int totalTopologyLinks = links.Count;

        var result = new TenantDashboardDto
        {
            TenantId = tenant.Id,
            TenantName = tenant.Name,
            IsActive = tenant.IsActive,
            CreatedAtUtc = tenant.CreatedAtUtc,
            TotalDevices = totalDevices,
            OnlineDevices = onlineDevices,
            OfflineDevices = offlineDevices,
            ActiveAlerts = activeAlertsCount,
            AverageHealthScore = averageHealthScore,
            TotalTopologyLinks = totalTopologyLinks,
            GeneratedAtUtc = DateTime.UtcNow
        };

        if (_cacheService != null)
        {
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromSeconds(30), cancellationToken);
        }

        return result;
    }
}