using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Telemetry;

public class TelemetryEngine : ITelemetryEngine
{
    public IEnumerable<DeviceMetricRaw> ProcessPollResult(SnmpPollResult pollResult)
    {
        var metrics = new List<DeviceMetricRaw>();

        if (!pollResult.IsSuccess || pollResult.OidValues.Count == 0)
        {
            return metrics;
        }

        // Extract CPU metric from SNMP response or default to 0
        decimal cpu = 0m;
        if (pollResult.OidValues.TryGetValue(OidConstants.CiscoCpu5Min, out var cpuStr) && decimal.TryParse(cpuStr, out var parsedCpu))
        {
            cpu = parsedCpu;
        }

        // Extract RAM metric or calculate from used/free
        decimal ram = 0m;
        if (pollResult.OidValues.TryGetValue(OidConstants.HostMemoryUsed, out var ramStr) && decimal.TryParse(ramStr, out var parsedRam))
        {
            ram = parsedRam;
        }

        int latency = (int)pollResult.ResponseTimeMs;

        var metricRecord = new DeviceMetricRaw(pollResult.DeviceId, cpu, ram, latency);
        metrics.Add(metricRecord);

        return metrics;
    }
}