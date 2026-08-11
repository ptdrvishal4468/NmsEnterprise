using MediatR;
using Nms.Domain.Interfaces;

namespace Nms.Application.Alerts.Commands.DeleteAlertRule;

public class DeleteAlertRuleCommandHandler : IRequestHandler<DeleteAlertRuleCommand, bool>
{
    private readonly IAlertRuleRepository _ruleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAlertRuleCommandHandler(IAlertRuleRepository ruleRepository, IUnitOfWork unitOfWork)
    {
        _ruleRepository = ruleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteAlertRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await _ruleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (rule == null) return false;

        _ruleRepository.Remove(rule);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}