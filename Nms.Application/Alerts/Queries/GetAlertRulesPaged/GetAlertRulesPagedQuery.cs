using MediatR;
using Nms.Application.Alerts.Dtos;
using Nms.Application.Common.Models;

namespace Nms.Application.Alerts.Queries.GetAlertRulesPaged;

public record GetAlertRulesPagedQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? DeviceId = null) : IRequest<PagedResult<AlertRuleDto>>;