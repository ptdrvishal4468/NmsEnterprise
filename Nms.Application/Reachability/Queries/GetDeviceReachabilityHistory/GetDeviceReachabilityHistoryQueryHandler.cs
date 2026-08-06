using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Reachability.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Reachability.Queries.GetDeviceReachabilityHistory;

public sealed class GetDeviceReachabilityHistoryQueryHandler : IRequestHandler<GetDeviceReachabilityHistoryQuery, PagedResult<ReachabilityHistoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDeviceReachabilityHistoryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<ReachabilityHistoryDto>> Handle(GetDeviceReachabilityHistoryQuery request, CancellationToken cancellationToken)
    {
        var device = await _unitOfWork.Devices.GetByIdAsync(request.DeviceId, cancellationToken);
        if (device == null)
        {
            throw new KeyNotFoundException($"Device with ID '{request.DeviceId}' was not found.");
        }

        var (items, totalCount) = await _unitOfWork.ReachabilityHistories.GetPagedByDeviceIdAsync(
            request.DeviceId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(h => new ReachabilityHistoryDto(
            h.Id,
            h.DeviceId,
            h.Status,
            h.MinLatencyMs,
            h.MaxLatencyMs,
            h.AvgLatencyMs,
            h.CurrentLatencyMs,
            h.PacketsSent,
            h.PacketsReceived,
            h.PacketLossPercentage,
            h.TimestampUtc)).ToList();

        return new PagedResult<ReachabilityHistoryDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}