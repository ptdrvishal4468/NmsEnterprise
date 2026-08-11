using MediatR;
using Nms.Application.Alerts.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Alerts.Commands.UpdateAlertRule;

public class UpdateAlertRuleCommandHandler : IRequestHandler<UpdateAlertRuleCommand, AlertRuleDto>
{
    private readonly IAlertRuleRepository _ruleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAlertRuleCommandHandler(IAlertRuleRepository ruleRepository, IUnitOfWork unitOfWork)
    {
        _ruleRepository = ruleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AlertRuleDto> Handle(UpdateAlertRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await _ruleRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Alert rule with ID {request.Id} was not found.");

        rule.Update(
            request.Name,
            request.Description,
            request.MetricType,
            request.Operator,
            request.ThresholdValue,
            request.Severity,
            request.DeviceId);

        _ruleRepository.Update(rule);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AlertRuleDto(
            rule.Id,
            rule.TenantId,
            rule.Name,
            rule.Description,
            rule.MetricType,
            rule.Operator,
            rule.ThresholdValue,
            rule.Severity,
            rule.IsEnabled,
            rule.DeviceId,
            rule.CreatedAtUtc);
    }
}