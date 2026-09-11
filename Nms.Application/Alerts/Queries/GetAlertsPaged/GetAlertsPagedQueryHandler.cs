using MediatR;
using Nms.Application.Alerts.Dtos;
using Nms.Application.Common.Models;
using Nms.Domain.Interfaces;

namespace Nms.Application.Alerts.Queries.GetAlertsPaged;

public class GetAlertsPagedQueryHandler : IRequestHandler<GetAlertsPagedQuery, PagedResult<AlertDto>>
{
    private readonly IAlertRepository _alertRepository;

    public GetAlertsPagedQueryHandler(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task<PagedResult<AlertDto>> Handle(GetAlertsPagedQuery request, CancellationToken cancellationToken)
    {
        var pageIndex = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : (request.PageSize > 100 ? 100 : request.PageSize);

        var (items, totalCount) = await _alertRepository.GetPagedAsync(
            pageIndex,
            pageSize,
            request.DeviceId,
            request.State,
            request.Severity,
            cancellationToken);

        var dtos = items.Select(a => new AlertDto(
            a.Id,
            a.TenantId,
            a.AlertRuleId,
            a.DeviceId,
            a.MetricType,
            a.Severity,
            a.State,
            a.MetricValue,
            a.ThresholdValue,
            a.Message,
            a.TriggeredAtUtc,
            a.LastOccurredAtUtc,
            a.AcknowledgedAtUtc,
            a.AcknowledgedBy,
            a.SuppressedAtUtc,
            a.SuppressedBy,
            a.ResolvedAtUtc)).ToList();

        return new PagedResult<AlertDto>(dtos, totalCount, pageIndex, pageSize);
    }
}