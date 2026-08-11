using MediatR;
using Nms.Application.Alerts.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Alerts.Commands.AcknowledgeAlert;

public class AcknowledgeAlertCommandHandler : IRequestHandler<AcknowledgeAlertCommand, AlertDto>
{
    private readonly IAlertRepository _alertRepository;
    private readonly IGenericRepository<AlertHistory, Guid> _historyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AcknowledgeAlertCommandHandler(
        IAlertRepository alertRepository,
        IGenericRepository<AlertHistory, Guid> historyRepository,
        IUnitOfWork unitOfWork)
    {
        _alertRepository = alertRepository;
        _historyRepository = historyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AlertDto> Handle(AcknowledgeAlertCommand request, CancellationToken cancellationToken)
    {
        var alert = await _alertRepository.GetByIdAsync(request.AlertId, cancellationToken)
            ?? throw new KeyNotFoundException($"Alert with ID {request.AlertId} was not found.");

        var oldState = alert.State;
        alert.Acknowledge(request.User);

        _alertRepository.Update(alert);

        var history = new AlertHistory(
            alert.TenantId,
            alert.Id,
            oldState,
            alert.State,
            request.User,
            request.Note ?? "Alert acknowledged by user.");

        await _historyRepository.AddAsync(history, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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