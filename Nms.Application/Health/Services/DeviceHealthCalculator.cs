using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Application.Health.Services;

public class DeviceHealthCalculator : IDeviceHealthCalculator
{
    public DeviceHealthEvaluationResult CalculateHealth(
        Device device,
        DeviceReachabilityHistory? latestReachability,
        DeviceMetricRaw? latestMetric,
        IReadOnlyList<NetworkInterface> interfaces)
    {
        var reasons = new List<string>();

        // 1. Reachability Component (Weight: 40%)
        double reachabilityScore = 100.0;

        if (latestReachability == null)
        {
            reachabilityScore = 50.0;
            reasons.Add("No reachability data available");
        }
        else if (latestReachability.PacketLossPercentage >= 100.0 || latestReachability.Status == ReachabilityStatus.Unreachable)
        {
            return new DeviceHealthEvaluationResult(
                0.0,
                DeviceStatus.Unreachable,
                "Device is completely unreachable (100% packet loss)");
        }
        else if (latestReachability.PacketLossPercentage > 0.0)
        {
            reachabilityScore = 40.0;
            reasons.Add($"Packet loss detected ({latestReachability.PacketLossPercentage:F1}%)");
        }
        else if (latestReachability.AvgLatencyMs > 100.0)
        {
            reachabilityScore = 70.0;
            reasons.Add($"High latency observed ({latestReachability.AvgLatencyMs:F1} ms)");
        }

        // 2. System Telemetry Component (Weight: 40%)
        double telemetryScore = 100.0;

        if (latestMetric != null)
        {
            if (latestMetric.CpuUtilization > 90.0m)
            {
                telemetryScore -= 50.0;
                reasons.Add($"Critical CPU usage ({latestMetric.CpuUtilization:F1}%)");
            }
            else if (latestMetric.CpuUtilization > 70.0m)
            {
                telemetryScore -= 25.0;
                reasons.Add($"High CPU usage ({latestMetric.CpuUtilization:F1}%)");
            }

            if (latestMetric.RamUtilization > 92.0m)
            {
                telemetryScore -= 50.0;
                reasons.Add($"Critical memory usage ({latestMetric.RamUtilization:F1}%)");
            }
            else if (latestMetric.RamUtilization > 80.0m)
            {
                telemetryScore -= 25.0;
                reasons.Add($"High memory usage ({latestMetric.RamUtilization:F1}%)");
            }
        }
        else
        {
            telemetryScore = 75.0;
            reasons.Add("No recent telemetry data available");
        }

        telemetryScore = Math.Clamp(telemetryScore, 0.0, 100.0);

        // 3. Interface Status Component (Weight: 20%)
        double interfaceScore = 100.0;
        if (interfaces != null && interfaces.Count > 0)
        {
            int totalInterfaces = interfaces.Count;
            int activeInterfaces = interfaces.Count(i => i.OperStatus == InterfaceOperStatus.Up);
            interfaceScore = ((double)activeInterfaces / totalInterfaces) * 100.0;

            if (activeInterfaces < totalInterfaces)
            {
                reasons.Add($"{totalInterfaces - activeInterfaces} of {totalInterfaces} interfaces down");
            }
        }

        // Weighted Final Calculation
        double finalScore = (reachabilityScore * 0.40) + (telemetryScore * 0.40) + (interfaceScore * 0.20);
        finalScore = Math.Round(Math.Clamp(finalScore, 0.0, 100.0), 2);

        DeviceStatus status = finalScore switch
        {
            >= 80.0 => DeviceStatus.Online,
            >= 50.0 => DeviceStatus.Degraded,
            > 0.0 => DeviceStatus.Degraded,
            _ => DeviceStatus.Offline
        };

        string summary = reasons.Count > 0
            ? string.Join("; ", reasons)
            : "Device operating within normal operational parameters";

        return new DeviceHealthEvaluationResult(finalScore, status, summary);
    }
}