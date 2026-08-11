using MediatR;
using Nms.Application.Alerts.Dtos;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Alerts.Commands.CreateAlertRule;

public class CreateAlertRuleCommandHandler : IRequestHandler<CreateAlertRuleCommand, AlertRuleDto>
{
    private readonly IAlertRuleRepository _ruleRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAlertRuleCommandHandler(
        IAlertRuleRepository ruleRepository,
        ITenantContext tenantContext,
        IUnitOfWork unitOfWork)
    {
        _ruleRepository = ruleRepository;
        _tenantContext = tenantContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<AlertRuleDto> Handle(CreateAlertRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = new AlertRule(
            _tenantContext.TenantId,
            request.Name,
            request.Description,
            request.MetricType,
            request.Operator,
            request.ThresholdValue,
            request.Severity,
            request.DeviceId);

        await _ruleRepository.AddAsync(rule, cancellationToken);
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