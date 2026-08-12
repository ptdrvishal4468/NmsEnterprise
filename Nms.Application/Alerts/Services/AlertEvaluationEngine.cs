using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Alerts.Services;

public class AlertEvaluationEngine : IAlertEvaluationEngine
{
    private readonly IAlertRuleRepository _ruleRepository;
    private readonly IAlertRepository _alertRepository;
    private readonly INotificationDispatcher _notificationDispatcher;
    private readonly IUnitOfWork _unitOfWork;

    public AlertEvaluationEngine(
        IAlertRuleRepository ruleRepository,
        IAlertRepository alertRepository,
        INotificationDispatcher notificationDispatcher,
        IUnitOfWork unitOfWork)
    {
        _ruleRepository = ruleRepository;
        _alertRepository = alertRepository;
        _notificationDispatcher = notificationDispatcher;
        _unitOfWork = unitOfWork;
    }

    public async Task EvaluateMetricsAsync(Guid tenantId, Guid deviceId, IEnumerable<DeviceMetricRaw> metrics, CancellationToken cancellationToken = default)
    {
        var rules = await _ruleRepository.GetActiveRulesForDeviceAsync(deviceId, cancellationToken);
        if (!rules.Any()) return;

        var latestMetric = metrics.OrderByDescending(m => m.TimestampUtc).FirstOrDefault();
        if (latestMetric == null) return;

        var alertsToNotify = new List<Alert>();

        foreach (var rule in rules)
        {
            decimal metricValue = GetMetricValueByType(latestMetric, rule.MetricType);
            bool isBreached = EvaluateThreshold(metricValue, rule.Operator, rule.ThresholdValue);

            var existingAlert = await _alertRepository.GetActiveAlertByRuleAndDeviceAsync(rule.Id, deviceId, cancellationToken);

            if (isBreached)
            {
                if (existingAlert == null)
                {
                    string message = $"Threshold breached for {rule.MetricType}: {metricValue} {GetOperatorSymbol(rule.Operator)} {rule.ThresholdValue}";
                    var alert = new Alert(
                        tenantId,
                        rule.Id,
                        deviceId,
                        rule.MetricType,
                        rule.Severity,
                        metricValue,
                        rule.ThresholdValue,
                        message);

                    await _alertRepository.AddAsync(alert, cancellationToken);
                    alertsToNotify.Add(alert);
                }
                else
                {
                    existingAlert.RecordRepeatedBreach(metricValue);
                    _alertRepository.Update(existingAlert);
                }
            }
            else
            {
                if (existingAlert != null && existingAlert.State != AlertState.Resolved)
                {
                    existingAlert.Resolve();
                    _alertRepository.Update(existingAlert);
                    alertsToNotify.Add(existingAlert);
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        foreach (var alert in alertsToNotify)
        {
            await _notificationDispatcher.DispatchAlertNotificationAsync(alert, cancellationToken);
        }
    }

    private static decimal GetMetricValueByType(DeviceMetricRaw metric, MetricType metricType)
    {
        return metricType switch
        {
            MetricType.CpuUsage => metric.CpuUtilization,
            MetricType.MemoryUsage => metric.RamUtilization,
            MetricType.DiskUsage => metric.DiskUtilization,
            MetricType.BandwidthIn => metric.InterfaceUtilization,
            MetricType.Latency => metric.LatencyMs,
            MetricType.Temperature => metric.Temperature,
            MetricType.FanStatus => metric.FanStatus,
            MetricType.PowerSupplyStatus => metric.PowerSupplyStatus,
            _ => 0m
        };
    }

    private static bool EvaluateThreshold(decimal value, ComparisonOperator op, decimal threshold)
    {
        return op switch
        {
            ComparisonOperator.GreaterThan => value > threshold,
            ComparisonOperator.GreaterThanOrEqual => value >= threshold,
            ComparisonOperator.LessThan => value < threshold,
            ComparisonOperator.LessThanOrEqual => value <= threshold,
            ComparisonOperator.Equal => value == threshold,
            ComparisonOperator.NotEqual => value != threshold,
            _ => false
        };
    }

    private static string GetOperatorSymbol(ComparisonOperator op) => op switch
    {
        ComparisonOperator.GreaterThan => ">",
        ComparisonOperator.GreaterThanOrEqual => ">=",
        ComparisonOperator.LessThan => "<",
        ComparisonOperator.LessThanOrEqual => "<=",
        ComparisonOperator.Equal => "==",
        ComparisonOperator.NotEqual => "!=",
        _ => "?"
    };
}