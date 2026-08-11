using MediatR;

namespace Nms.Application.Alerts.Commands.DeleteAlertRule;

public record DeleteAlertRuleCommand(Guid Id) : IRequest<bool>;