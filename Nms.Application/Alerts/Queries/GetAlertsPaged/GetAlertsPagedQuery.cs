using MediatR;
using Nms.Application.Alerts.Dtos;
using Nms.Application.Common.Models;
using Nms.Domain.Enums;

namespace Nms.Application.Alerts.Queries.GetAlertsPaged;

public record GetAlertsPagedQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? DeviceId = null,
    AlertState? State = null,
    AlertSeverity? Severity = null) : IRequest<PagedResult<AlertDto>>;