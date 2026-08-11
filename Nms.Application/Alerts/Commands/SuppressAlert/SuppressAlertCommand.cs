using MediatR;
using Nms.Application.Alerts.Dtos;

namespace Nms.Application.Alerts.Commands.SuppressAlert;

public record SuppressAlertCommand(Guid AlertId, string User, string? Note = null) : IRequest<AlertDto>;