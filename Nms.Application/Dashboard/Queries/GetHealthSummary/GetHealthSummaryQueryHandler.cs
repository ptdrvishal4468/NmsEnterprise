using MediatR;
using Nms.Application.Dashboard.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Dashboard.Queries.GetHealthSummary;

public class GetHealthSummaryQueryHandler : IRequestHandler<GetHealthSummaryQuery, HealthSummaryDto>
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IDeviceHealthHistoryRepository _healthHistoryRepository;

    public GetHealthSummaryQueryHandler(
        IDeviceRepository deviceRepository,
        IDeviceHealthHistoryRepository healthHistoryRepository)
    {
        _deviceRepository = deviceRepository;
        _healthHistoryRepository = healthHistoryRepository;
    }

    public async Task<HealthSummaryDto> Handle(GetHealthSummaryQuery request, CancellationToken cancellationToken)
    {
        var devices = await _deviceRepository.GetAllAsync(cancellationToken);
        int totalDevices = devices.Count;

        if (totalDevices == 0)
        {
            return new HealthSummaryDto
            {
                TotalMonitoredDevices = 0,
                AverageHealthScore = 100.0,
                HealthyDevicesCount = 0,
                WarningDevicesCount = 0,
                CriticalDevicesCount = 0,
                TopDegradedDevices = Array.Empty<DegradedDeviceHealthDto>(),
                GeneratedAtUtc = DateTime.UtcNow
            };
        }

        var deviceLookup = devices.ToDictionary(d => d.Id);
        var recentCutoff = DateTime.UtcNow.AddHours(-24);
        var recentHistories = await _healthHistoryRepository.FindAsync(
            h => h.TimestampUtc >= recentCutoff,
            cancellationToken);

        // Group by device to extract the latest evaluation for each device
        var latestPerDevice = recentHistories
            .GroupBy(h => h.DeviceId)
            .Select(g => g.OrderByDescending(h => h.TimestampUtc).First())
            .ToList();

        int healthyCount = 0;
        int warningCount = 0;
        int criticalCount = 0;
        var degradedList = new List<DegradedDeviceHealthDto>();
        double averageScore;

        if (latestPerDevice.Count > 0)
        {
            foreach (var record in latestPerDevice)
            {
                if (!deviceLookup.TryGetValue(record.DeviceId, out var device))
                    continue;

                if (record.HealthScore >= 80.0)
                {
                    healthyCount++;
                }
                else if (record.HealthScore >= 50.0)
                {
                    warningCount++;
                    degradedList.Add(new DegradedDeviceHealthDto(
                        device.Id,
                        device.Name,
                        device.IpAddress,
                        record.HealthScore,
                        record.Status.ToString(),
                        record.Reason,
                        record.TimestampUtc));
                }
                else
                {
                    criticalCount++;
                    degradedList.Add(new DegradedDeviceHealthDto(
                        device.Id,
                        device.Name,
                        device.IpAddress,
                        record.HealthScore,
                        record.Status.ToString(),
                        record.Reason,
                        record.TimestampUtc));
                }
            }

            // Include remaining devices without recent history as healthy by default if online
            int unrecordedDevices = totalDevices - latestPerDevice.Count;
            if (unrecordedDevices > 0)
            {
                healthyCount += unrecordedDevices;
            }

            averageScore = Math.Round(latestPerDevice.Average(h => h.HealthScore), 2);
        }
        else
        {
            // Fallback approximation when health history table is empty
            foreach (var device in devices)
            {
                switch (device.Status)
                {
                    case DeviceStatus.Online:
                    case DeviceStatus.Unknown:
                        healthyCount++;
                        break;
                    case DeviceStatus.Degraded:
                        warningCount++;
                        degradedList.Add(new DegradedDeviceHealthDto(
                            device.Id,
                            device.Name,
                            device.IpAddress,
                            60.0,
                            device.Status.ToString(),
                            "Device operating in degraded condition.",
                            device.LastSeenUtc ?? DateTime.UtcNow));
                        break;
                    case DeviceStatus.Unreachable:
                    case DeviceStatus.Offline:
                        criticalCount++;
                        degradedList.Add(new DegradedDeviceHealthDto(
                            device.Id,
                            device.Name,
                            device.IpAddress,
                            device.Status == DeviceStatus.Unreachable ? 20.0 : 0.0,
                            device.Status.ToString(),
                            "Device is offline or unreachable.",
                            device.LastSeenUtc ?? DateTime.UtcNow));
                        break;
                }
            }

            double scoreSum = (healthyCount * 100.0) + (warningCount * 60.0) + (criticalCount * 20.0);
            averageScore = Math.Round(scoreSum / totalDevices, 2);
        }

        var topDegraded = degradedList
            .OrderBy(d => d.HealthScore)
            .Take(Math.Max(1, request.TopDegradedLimit))
            .ToList();

        return new HealthSummaryDto
        {
            TotalMonitoredDevices = totalDevices,
            AverageHealthScore = averageScore,
            HealthyDevicesCount = healthyCount,
            WarningDevicesCount = warningCount,
            CriticalDevicesCount = criticalCount,
            TopDegradedDevices = topDegraded,
            GeneratedAtUtc = DateTime.UtcNow
        };
    }
}