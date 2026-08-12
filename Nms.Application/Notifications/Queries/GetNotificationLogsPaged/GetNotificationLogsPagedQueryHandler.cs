using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Application.Notifications.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Notifications.Queries.GetNotificationLogsPaged;

public class GetNotificationLogsPagedQueryHandler : IRequestHandler<GetNotificationLogsPagedQuery, PagedResult<NotificationLogDto>>
{
    private readonly INotificationLogRepository _logRepository;
    private readonly ITenantContext _tenantContext;

    public GetNotificationLogsPagedQueryHandler(
        INotificationLogRepository logRepository,
        ITenantContext tenantContext)
    {
        _logRepository = logRepository;
        _tenantContext = tenantContext;
    }

    public async Task<PagedResult<NotificationLogDto>> Handle(GetNotificationLogsPagedQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _logRepository.GetPagedLogsAsync(
            _tenantContext.TenantId,
            request.PageIndex,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(l => new NotificationLogDto(
            l.Id,
            l.TenantId,
            l.AlertId,
            l.Channel,
            l.RecipientTarget,
            l.Subject,
            l.Message,
            l.Status,
            l.ErrorMessage,
            l.SentAtUtc));

        return new PagedResult<NotificationLogDto>(dtos.ToList(), totalCount, request.PageIndex, request.PageSize);
    }
}