using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Application.Notifications.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Notifications.Queries.GetNotificationTemplatesPaged;

public class GetNotificationTemplatesPagedQueryHandler : IRequestHandler<GetNotificationTemplatesPagedQuery, PagedResult<NotificationTemplateDto>>
{
    private readonly INotificationTemplateRepository _templateRepository;
    private readonly ITenantContext _tenantContext;

    public GetNotificationTemplatesPagedQueryHandler(
        INotificationTemplateRepository templateRepository,
        ITenantContext tenantContext)
    {
        _templateRepository = templateRepository;
        _tenantContext = tenantContext;
    }

    public async Task<PagedResult<NotificationTemplateDto>> Handle(GetNotificationTemplatesPagedQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _templateRepository.GetPagedTemplatesAsync(
            _tenantContext.TenantId,
            request.PageIndex,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(t => new NotificationTemplateDto(
            t.Id,
            t.TenantId,
            t.Name,
            t.Channel,
            t.SubjectTemplate,
            t.BodyTemplate,
            t.RecipientTarget,
            t.IsEnabled,
            t.CreatedAtUtc,
            t.LastModifiedAtUtc));

        return new PagedResult<NotificationTemplateDto>(dtos.ToList(), totalCount, request.PageIndex, request.PageSize);
    }
}