using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Application.Common.Interfaces;

public record DeviceHealthEvaluationResult(
    double HealthScore,
    DeviceStatus Status,
    string SummaryReason);

public interface IDeviceHealthCalculator
{
    DeviceHealthEvaluationResult CalculateHealth(
        Device device,
        DeviceReachabilityHistory? latestReachability,
        DeviceMetricRaw? latestMetric,
        IReadOnlyList<NetworkInterface> interfaces);
}