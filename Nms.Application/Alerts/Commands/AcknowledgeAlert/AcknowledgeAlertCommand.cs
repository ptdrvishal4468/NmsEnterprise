using MediatR;
using Nms.Application.Alerts.Dtos;

namespace Nms.Application.Alerts.Commands.AcknowledgeAlert;

public record AcknowledgeAlertCommand(Guid AlertId, string User, string? Note = null) : IRequest<AlertDto>;