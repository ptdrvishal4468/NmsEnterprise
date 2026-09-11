using MediatR;
using Nms.Application.Common.Interfaces;

namespace Nms.Application.Alerts.Commands.EvaluateTelemetryAlerts;

public class EvaluateTelemetryAlertsCommandHandler : IRequestHandler<EvaluateTelemetryAlertsCommand>
{
    private readonly IAlertEvaluationEngine _evaluationEngine;

    public EvaluateTelemetryAlertsCommandHandler(IAlertEvaluationEngine evaluationEngine)
    {
        _evaluationEngine = evaluationEngine;
    }

    public async Task Handle(EvaluateTelemetryAlertsCommand request, CancellationToken cancellationToken)
    {
        await _evaluationEngine.EvaluateMetricsAsync(
            request.TenantId,
            request.DeviceId,
            request.Metrics,
            cancellationToken);
    }
}