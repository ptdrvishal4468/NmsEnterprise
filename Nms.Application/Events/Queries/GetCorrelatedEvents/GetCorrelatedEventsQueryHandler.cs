using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Events.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Events.Queries.GetCorrelatedEvents;

public class GetCorrelatedEventsQueryHandler : IRequestHandler<GetCorrelatedEventsQuery, IReadOnlyList<EventDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetCorrelatedEventsQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<IReadOnlyList<EventDto>> Handle(GetCorrelatedEventsQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.Events.GetCorrelatedEventsAsync(
            tenantId: _tenantContext.TenantId,
            correlationId: request.CorrelationId,
            cancellationToken: cancellationToken);

        return items.Select(EventDto.FromEntity).ToList();
    }
}