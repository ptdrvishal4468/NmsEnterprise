using MediatR;
using Nms.Application.Alerts.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Alerts.Commands.UpdateAlertRule;

public record UpdateAlertRuleCommand(
    Guid Id,
    string Name,
    string Description,
    MetricType MetricType,
    ComparisonOperator Operator,
    decimal ThresholdValue,
    AlertSeverity Severity,
    Guid? DeviceId = null) : IRequest<AlertRuleDto>;