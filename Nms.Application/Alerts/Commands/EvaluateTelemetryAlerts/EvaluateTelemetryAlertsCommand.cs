using MediatR;
using Nms.Domain.Entities;

namespace Nms.Application.Alerts.Commands.EvaluateTelemetryAlerts;

public record EvaluateTelemetryAlertsCommand(
    Guid TenantId,
    Guid DeviceId,
    IEnumerable<DeviceMetricRaw> Metrics) : IRequest;