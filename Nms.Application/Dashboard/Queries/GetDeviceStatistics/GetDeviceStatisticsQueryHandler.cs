using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Dashboard.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Dashboard.Queries.GetDeviceStatistics;

public class GetDeviceStatisticsQueryHandler : IRequestHandler<GetDeviceStatisticsQuery, DeviceStatisticsDto>
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly ICacheService? _cacheService;

    public GetDeviceStatisticsQueryHandler(
        IDeviceRepository deviceRepository,
        ICacheService? cacheService = null)
    {
        _deviceRepository = deviceRepository;
        _cacheService = cacheService;
    }

    public async Task<DeviceStatisticsDto> Handle(GetDeviceStatisticsQuery request, CancellationToken cancellationToken)
    {
        const string cacheKey = "dashboard:device-statistics";
        if (_cacheService != null)
        {
            var cached = await _cacheService.GetAsync<DeviceStatisticsDto>(cacheKey, cancellationToken);
            if (cached != null)
            {
                return cached;
            }
        }

        var devices = await _deviceRepository.GetAllAsync(cancellationToken);
        int totalDevices = devices.Count;

        // 1. Status Distribution
        var statusDistribution = devices
            .GroupBy(d => d.Status.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        // 2. Device Type Distribution
        var typeDistribution = devices
            .GroupBy(d => d.DeviceType.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        // 3. Vendor Distribution (normalized for null/whitespace)
        var vendorDistribution = devices
            .GroupBy(d => string.IsNullOrWhiteSpace(d.Vendor) ? "Unassigned" : d.Vendor.Trim())
            .ToDictionary(g => g.Key, g => g.Count());

        // 4. Site Distribution (normalized for null/whitespace)
        var siteDistribution = devices
            .GroupBy(d => string.IsNullOrWhiteSpace(d.Site) ? "Unassigned" : d.Site.Trim())
            .ToDictionary(g => g.Key, g => g.Count());

        var result = new DeviceStatisticsDto
        {
            TotalDevices = totalDevices,
            StatusDistribution = statusDistribution,
            TypeDistribution = typeDistribution,
            VendorDistribution = vendorDistribution,
            SiteDistribution = siteDistribution,
            GeneratedAtUtc = DateTime.UtcNow
        };

        if (_cacheService != null)
        {
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromSeconds(30), cancellationToken);
        }

        return result;
    }
}