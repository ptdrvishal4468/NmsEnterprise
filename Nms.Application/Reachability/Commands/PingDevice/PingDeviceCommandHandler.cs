using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Reachability.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Reachability.Commands.PingDevice;

public sealed class PingDeviceCommandHandler : IRequestHandler<PingDeviceCommand, PingSummaryDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIcmpPingService _icmpPingService;

    public PingDeviceCommandHandler(IUnitOfWork unitOfWork, IIcmpPingService icmpPingService)
    {
        _unitOfWork = unitOfWork;
        _icmpPingService = icmpPingService;
    }

    public async Task<PingSummaryDto> Handle(PingDeviceCommand request, CancellationToken cancellationToken)
    {
        var device = await _unitOfWork.Devices.GetByIdAsync(request.DeviceId, cancellationToken);
        if (device == null)
        {
            throw new KeyNotFoundException($"Device with ID '{request.DeviceId}' was not found.");
        }

        var pingResult = await _icmpPingService.PingAsync(
            device.IpAddress,
            request.PacketCount,
            request.TimeoutMs,
            cancellationToken);

        // Map micro-level ReachabilityStatus to macro-level DeviceStatus
        var newDeviceStatus = pingResult.Status switch
        {
            ReachabilityStatus.Online => DeviceStatus.Online,
            ReachabilityStatus.Offline => DeviceStatus.Offline,
            ReachabilityStatus.Timeout => DeviceStatus.Offline,
            ReachabilityStatus.Unreachable => DeviceStatus.Unreachable,
            _ => DeviceStatus.Unknown
        };

        device.UpdateStatus(newDeviceStatus);

        var executionTime = DateTime.UtcNow;
        var history = new DeviceReachabilityHistory(
            Guid.NewGuid(),
            device.Id,
            device.TenantId,
            pingResult.Status,
            pingResult.MinLatencyMs,
            pingResult.MaxLatencyMs,
            pingResult.AvgLatencyMs,
            pingResult.CurrentLatencyMs,
            pingResult.PacketsSent,
            pingResult.PacketsReceived,
            pingResult.PacketLossPercentage,
            executionTime);

        await _unitOfWork.ReachabilityHistories.AddAsync(history, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PingSummaryDto(
            device.Id,
            pingResult.Status,
            pingResult.MinLatencyMs,
            pingResult.MaxLatencyMs,
            pingResult.AvgLatencyMs,
            pingResult.CurrentLatencyMs,
            pingResult.PacketsSent,
            pingResult.PacketsReceived,
            pingResult.PacketLossPercentage,
            executionTime);
    }
}