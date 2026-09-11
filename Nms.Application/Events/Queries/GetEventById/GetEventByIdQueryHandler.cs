using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Events.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Events.Queries.GetEventById;

public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetEventByIdQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<EventDto?> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var deviceEvent = await _unitOfWork.Events.GetByIdAsync(request.Id, cancellationToken);
        if (deviceEvent == null || deviceEvent.TenantId != _tenantContext.TenantId)
        {
            return null;
        }

        return EventDto.FromEntity(deviceEvent);
    }
}