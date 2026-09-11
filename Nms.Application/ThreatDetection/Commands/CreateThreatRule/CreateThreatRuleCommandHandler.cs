using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.ThreatDetection.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.ThreatDetection.Commands.CreateThreatRule;

public class CreateThreatRuleCommandHandler : IRequestHandler<CreateThreatRuleCommand, ThreatDetectionRuleDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly ICacheService? _cacheService;

    public CreateThreatRuleCommandHandler(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext,
        ICacheService? cacheService = null)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _cacheService = cacheService;
    }

    public async Task<ThreatDetectionRuleDto> Handle(CreateThreatRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = new ThreatDetectionRule
        {
            TenantId = _tenantContext.TenantId,
            RuleName = request.RuleName,
            ThreatType = request.ThreatType,
            DefaultSeverity = request.DefaultSeverity,
            FailureThreshold = request.FailureThreshold,
            TimeWindowMinutes = request.TimeWindowMinutes,
            IsEnabled = request.IsEnabled,
            Description = request.Description
        };

        await _unitOfWork.ThreatDetectionRules.AddAsync(rule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (_cacheService != null)
        {
            await _cacheService.RemoveByPrefixAsync("rules:threats", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("dashboard:threats", cancellationToken);
        }

        return new ThreatDetectionRuleDto
        {
            Id = rule.Id,
            TenantId = rule.TenantId,
            RuleName = rule.RuleName,
            ThreatType = rule.ThreatType,
            DefaultSeverity = rule.DefaultSeverity,
            FailureThreshold = rule.FailureThreshold,
            TimeWindowMinutes = rule.TimeWindowMinutes,
            IsEnabled = rule.IsEnabled,
            Description = rule.Description
        };
    }
}