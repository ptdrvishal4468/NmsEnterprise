using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Health.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Health.Queries.GetDeviceHealthHistory;

public class GetDeviceHealthHistoryQueryHandler : IRequestHandler<GetDeviceHealthHistoryQuery, PagedResult<DeviceHealthHistoryDto>>
{
    private readonly IDeviceHealthHistoryRepository _healthHistoryRepository;

    public GetDeviceHealthHistoryQueryHandler(IDeviceHealthHistoryRepository healthHistoryRepository)
    {
        _healthHistoryRepository = healthHistoryRepository;
    }

    public async Task<PagedResult<DeviceHealthHistoryDto>> Handle(GetDeviceHealthHistoryQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _healthHistoryRepository.GetPagedByDeviceIdAsync(
            request.DeviceId,
            request.PageIndex,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(h => new DeviceHealthHistoryDto(
            h.Id,
            h.DeviceId,
            h.HealthScore,
            h.Status,
            h.Reason,
            h.TimestampUtc)).ToList();

        return new PagedResult<DeviceHealthHistoryDto>(dtos, totalCount, request.PageIndex, request.PageSize);
    }
}