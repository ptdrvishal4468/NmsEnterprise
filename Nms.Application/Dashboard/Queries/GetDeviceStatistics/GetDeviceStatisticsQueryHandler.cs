using MediatR;
using Nms.Application.Dashboard.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Dashboard.Queries.GetDeviceStatistics;

public class GetDeviceStatisticsQueryHandler : IRequestHandler<GetDeviceStatisticsQuery, DeviceStatisticsDto>
{
    private readonly IDeviceRepository _deviceRepository;

    public GetDeviceStatisticsQueryHandler(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<DeviceStatisticsDto> Handle(GetDeviceStatisticsQuery request, CancellationToken cancellationToken)
    {
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

        return new DeviceStatisticsDto
        {
            TotalDevices = totalDevices,
            StatusDistribution = statusDistribution,
            TypeDistribution = typeDistribution,
            VendorDistribution = vendorDistribution,
            SiteDistribution = siteDistribution,
            GeneratedAtUtc = DateTime.UtcNow
        };
    }
}