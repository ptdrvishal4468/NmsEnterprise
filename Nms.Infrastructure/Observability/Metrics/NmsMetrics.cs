using System.Diagnostics.Metrics;
using Nms.Infrastructure.Observability.Diagnostics;

namespace Nms.Infrastructure.Observability.Metrics;

public static class NmsMetrics
{
    private static readonly Counter<long> PollingJobsEnqueuedCounter =
        NmsDiagnostics.Meter.CreateCounter<long>(
            "nms.polling.jobs.enqueued",
            unit: "{jobs}",
            description: "Total number of polling jobs enqueued.");

    private static readonly Counter<long> PollingJobsProcessedCounter =
        NmsDiagnostics.Meter.CreateCounter<long>(
            "nms.polling.jobs.processed",
            unit: "{jobs}",
            description: "Total number of polling jobs processed.");

    private static readonly Histogram<double> PollingJobDurationHistogram =
        NmsDiagnostics.Meter.CreateHistogram<double>(
            "nms.polling.job.duration",
            unit: "ms",
            description: "Duration of polling job execution in milliseconds.");

    private static readonly Counter<long> AlertsEvaluatedCounter =
        NmsDiagnostics.Meter.CreateCounter<long>(
            "nms.alerts.evaluated",
            unit: "{evaluations}",
            description: "Total number of alert rule evaluations.");

    private static readonly Counter<long> AlertsTriggeredCounter =
        NmsDiagnostics.Meter.CreateCounter<long>(
            "nms.alerts.triggered",
            unit: "{alerts}",
            description: "Total number of alerts generated.");

    private static readonly Counter<long> ReachabilityPingsCounter =
        NmsDiagnostics.Meter.CreateCounter<long>(
            "nms.reachability.pings",
            unit: "{pings}",
            description: "Total ICMP reachability pings executed.");

    private static readonly Histogram<double> ReachabilityLatencyHistogram =
        NmsDiagnostics.Meter.CreateHistogram<double>(
            "nms.reachability.latency",
            unit: "ms",
            description: "ICMP reachability latency in milliseconds.");

    public static void RecordPollingJobEnqueued(string jobType)
    {
        PollingJobsEnqueuedCounter.Add(1, new KeyValuePair<string, object?>("job.type", jobType));
    }

    public static void RecordPollingJobProcessed(string jobType, bool success, double durationMs)
    {
        var status = success ? "success" : "failure";
        PollingJobsProcessedCounter.Add(1,
            new KeyValuePair<string, object?>("job.type", jobType),
            new KeyValuePair<string, object?>("status", status));
        PollingJobDurationHistogram.Record(durationMs,
            new KeyValuePair<string, object?>("job.type", jobType),
            new KeyValuePair<string, object?>("status", status));
    }

    public static void RecordAlertEvaluation(int count = 1)
    {
        AlertsEvaluatedCounter.Add(count);
    }

    public static void RecordAlertTriggered(string severity)
    {
        AlertsTriggeredCounter.Add(1, new KeyValuePair<string, object?>("severity", severity));
    }

    public static void RecordReachabilityPing(bool success, double latencyMs)
    {
        var status = success ? "online" : "offline";
        ReachabilityPingsCounter.Add(1, new KeyValuePair<string, object?>("status", status));
        if (success)
        {
            ReachabilityLatencyHistogram.Record(latencyMs, new KeyValuePair<string, object?>("status", status));
        }
    }
}