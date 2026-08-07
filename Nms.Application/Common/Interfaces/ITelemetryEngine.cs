using Nms.Application.Common.Models;
using Nms.Domain.Entities;

namespace Nms.Application.Common.Interfaces;

public interface ITelemetryEngine
{
    IEnumerable<DeviceMetricRaw> ProcessPollResult(Guid tenantId, SnmpPollResult pollResult);
}