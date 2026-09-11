using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Telemetry;

public class TelemetryEngine : ITelemetryEngine
{
    public IEnumerable<DeviceMetricRaw> ProcessPollResult(Guid tenantId, SnmpPollResult pollResult)
    {
        var metrics = new List<DeviceMetricRaw>();

        if (!pollResult.IsSuccess || pollResult.OidValues.Count == 0)
        {
            return metrics;
        }

        decimal cpu = ExtractDecimal(pollResult, OidConstants.CiscoCpu5Min);
        decimal ram = ExtractDecimal(pollResult, OidConstants.HostMemoryUsed);
        decimal disk = ExtractDecimal(pollResult, OidConstants.DiskUtilization);
        decimal interfaceUtil = ExtractDecimal(pollResult, OidConstants.InterfaceUtilization);
        decimal temp = ExtractDecimal(pollResult, OidConstants.CiscoEnvMonTemperature);
        int fanStatus = ExtractInt(pollResult, OidConstants.CiscoEnvMonFanStatus, defaultValue: 1);
        int powerSupplyStatus = ExtractInt(pollResult, OidConstants.CiscoEnvMonSupplyStatus, defaultValue: 1);
        int latency = (int)pollResult.ResponseTimeMs;

        var metricRecord = new DeviceMetricRaw(
            tenantId,
            pollResult.DeviceId,
            cpu,
            ram,
            disk,
            interfaceUtil,
            temp,
            fanStatus,
            powerSupplyStatus,
            latency
        );

        metrics.Add(metricRecord);
        return metrics;
    }

    private static decimal ExtractDecimal(SnmpPollResult pollResult, string oid)
    {
        if (pollResult.OidValues.TryGetValue(oid, out var strVal) && decimal.TryParse(strVal, out var val))
        {
            return val;
        }
        return 0m;
    }

    private static int ExtractInt(SnmpPollResult pollResult, string oid, int defaultValue)
    {
        if (pollResult.OidValues.TryGetValue(oid, out var strVal) && int.TryParse(strVal, out var val))
        {
            return val;
        }
        return defaultValue;
    }
}