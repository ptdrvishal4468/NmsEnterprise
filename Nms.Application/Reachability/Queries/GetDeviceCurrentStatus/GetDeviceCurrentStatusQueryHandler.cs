using MediatR;
using Nms.Application.Reachability.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Reachability.Queries.GetDeviceCurrentStatus;

public sealed class GetDeviceCurrentStatusQueryHandler : IRequestHandler<GetDeviceCurrentStatusQuery, PingSummaryDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDeviceCurrentStatusQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PingSummaryDto> Handle(GetDeviceCurrentStatusQuery request, CancellationToken cancellationToken)
    {
        var device = await _unitOfWork.Devices.GetByIdAsync(request.DeviceId, cancellationToken);
        if (device == null)
        {
            throw new KeyNotFoundException($"Device with ID '{request.DeviceId}' was not found.");
        }

        var latestHistory = await _unitOfWork.ReachabilityHistories.GetLatestByDeviceIdAsync(request.DeviceId, cancellationToken);

        if (latestHistory == null)
        {
            return new PingSummaryDto(
                device.Id,
                ReachabilityStatus.Unknown,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                DateTime.UtcNow);
        }

        return new PingSummaryDto(
            latestHistory.DeviceId,
            latestHistory.Status,
            latestHistory.MinLatencyMs,
            latestHistory.MaxLatencyMs,
            latestHistory.AvgLatencyMs,
            latestHistory.CurrentLatencyMs,
            latestHistory.PacketsSent,
            latestHistory.PacketsReceived,
            latestHistory.PacketLossPercentage,
            latestHistory.TimestampUtc);
    }
}