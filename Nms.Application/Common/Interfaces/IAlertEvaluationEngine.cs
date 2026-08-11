using Nms.Domain.Entities;

namespace Nms.Application.Common.Interfaces;

public interface IAlertEvaluationEngine
{
    Task EvaluateMetricsAsync(Guid tenantId, Guid deviceId, IEnumerable<DeviceMetricRaw> metrics, CancellationToken cancellationToken = default);
}