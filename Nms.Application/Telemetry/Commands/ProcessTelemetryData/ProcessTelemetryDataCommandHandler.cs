using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Telemetry.Commands.ProcessTelemetryData;

public class ProcessTelemetryDataCommandHandler : IRequestHandler<ProcessTelemetryDataCommand, bool>
{
    private readonly ITelemetryEngine _telemetryEngine;
    private readonly IDeviceMetricRepository _metricRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAlertEvaluationEngine _alertEvaluationEngine;

    public ProcessTelemetryDataCommandHandler(
        ITelemetryEngine telemetryEngine,
        IDeviceMetricRepository metricRepository,
        ITenantContext tenantContext,
        IUnitOfWork unitOfWork,
        IAlertEvaluationEngine alertEvaluationEngine)
    {
        _telemetryEngine = telemetryEngine;
        _metricRepository = metricRepository;
        _tenantContext = tenantContext;
        _unitOfWork = unitOfWork;
        _alertEvaluationEngine = alertEvaluationEngine;
    }

    public async Task<bool> Handle(ProcessTelemetryDataCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        IEnumerable<DeviceMetricRaw> metrics = _telemetryEngine.ProcessPollResult(tenantId, request.PollResult);

        if (!metrics.Any())
        {
            return false;
        }

        await _metricRepository.AddBulkAsync(metrics, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Trigger Phase 30 Alert Engine threshold evaluation
        await _alertEvaluationEngine.EvaluateMetricsAsync(
            tenantId,
            request.PollResult.DeviceId,
            metrics,
            cancellationToken);

        return true;
    }
}