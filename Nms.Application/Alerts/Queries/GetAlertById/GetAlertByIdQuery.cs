using MediatR;
using Nms.Application.Alerts.Dtos;

namespace Nms.Application.Alerts.Queries.GetAlertById;

public record GetAlertByIdQuery(Guid Id) : IRequest<AlertDto?>;