using MediatR;
using Nms.Application.Alerts.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Alerts.Queries.GetAlertById;

public class GetAlertByIdQueryHandler : IRequestHandler<GetAlertByIdQuery, AlertDto?>
{
    private readonly IAlertRepository _alertRepository;

    public GetAlertByIdQueryHandler(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task<AlertDto?> Handle(GetAlertByIdQuery request, CancellationToken cancellationToken)
    {
        var alert = await _alertRepository.GetByIdAsync(request.Id, cancellationToken);
        if (alert == null) return null;

        return new AlertDto(
            alert.Id,
            alert.TenantId,
            alert.AlertRuleId,
            alert.DeviceId,
            alert.MetricType,
            alert.Severity,
            alert.State,
            alert.MetricValue,
            alert.ThresholdValue,
            alert.Message,
            alert.TriggeredAtUtc,
            alert.LastOccurredAtUtc,
            alert.AcknowledgedAtUtc,
            alert.AcknowledgedBy,
            alert.SuppressedAtUtc,
            alert.SuppressedBy,
            alert.ResolvedAtUtc);
    }
}