using MediatR;
using Nms.Application.Alerts.Dtos;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Interfaces;

namespace Nms.Application.Alerts.Commands.UpdateAlertRule;

public class UpdateAlertRuleCommandHandler : IRequestHandler<UpdateAlertRuleCommand, AlertRuleDto>
{
    private readonly IAlertRuleRepository _ruleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService? _cacheService;

    public UpdateAlertRuleCommandHandler(
        IAlertRuleRepository ruleRepository,
        IUnitOfWork unitOfWork,
        ICacheService? cacheService = null)
    {
        _ruleRepository = ruleRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
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

        if (_cacheService != null)
        {
            await _cacheService.RemoveByPrefixAsync("rules:alerts", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("dashboard:alerts", cancellationToken);
        }

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