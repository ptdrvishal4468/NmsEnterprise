using MediatR;
using Nms.Application.Reachability.Dtos;

namespace Nms.Application.Reachability.Commands.PingDevice;

public sealed record PingDeviceCommand(
    Guid DeviceId,
    int PacketCount = 4,
    int TimeoutMs = 1000) : IRequest<PingSummaryDto>;