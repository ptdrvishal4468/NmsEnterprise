using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Telemetry.Commands.ProcessTelemetryData;

public class ProcessTelemetryDataCommandHandler : IRequestHandler<ProcessTelemetryDataCommand, bool>
{
    private readonly ITelemetryEngine _telemetryEngine;
    private readonly IDeviceMetricRepository _metricRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessTelemetryDataCommandHandler(
        ITelemetryEngine telemetryEngine,
        IDeviceMetricRepository metricRepository,
        IUnitOfWork unitOfWork)
    {
        _telemetryEngine = telemetryEngine;
        _metricRepository = metricRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ProcessTelemetryDataCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<DeviceMetricRaw> metrics = _telemetryEngine.ProcessPollResult(request.PollResult);

        if (!metrics.Any())
        {
            return false;
        }

        await _metricRepository.AddBulkAsync(metrics, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}