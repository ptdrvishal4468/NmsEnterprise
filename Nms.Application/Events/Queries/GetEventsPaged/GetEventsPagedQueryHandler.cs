using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Application.Events.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Events.Queries.GetEventsPaged;

public class GetEventsPagedQueryHandler : IRequestHandler<GetEventsPagedQuery, PagedResult<EventDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetEventsPagedQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<PagedResult<EventDto>> Handle(GetEventsPagedQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.Events.GetEventsPagedAsync(
            tenantId: _tenantContext.TenantId,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            deviceId: request.DeviceId,
            category: request.Category,
            severity: request.Severity,
            correlationId: request.CorrelationId,
            fromUtc: request.FromUtc,
            toUtc: request.ToUtc,
            cancellationToken: cancellationToken);

        var dtos = items.Select(EventDto.FromEntity).ToList();

        return new PagedResult<EventDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}