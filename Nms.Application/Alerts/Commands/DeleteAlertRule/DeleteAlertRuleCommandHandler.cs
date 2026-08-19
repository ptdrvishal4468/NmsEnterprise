using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Interfaces;

namespace Nms.Application.Alerts.Commands.DeleteAlertRule;

public class DeleteAlertRuleCommandHandler : IRequestHandler<DeleteAlertRuleCommand, bool>
{
    private readonly IAlertRuleRepository _ruleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService? _cacheService;

    public DeleteAlertRuleCommandHandler(
        IAlertRuleRepository ruleRepository,
        IUnitOfWork unitOfWork,
        ICacheService? cacheService = null)
    {
        _ruleRepository = ruleRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(DeleteAlertRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await _ruleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (rule == null) return false;

        _ruleRepository.Remove(rule);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (_cacheService != null)
        {
            await _cacheService.RemoveByPrefixAsync("rules:alerts", cancellationToken);
            await _cacheService.RemoveByPrefixAsync("dashboard:alerts", cancellationToken);
        }

        return true;
    }
}