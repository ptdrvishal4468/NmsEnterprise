using MediatR;
using Nms.Application.Devices.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Devices.Commands.TestDeviceConnectivity;

public sealed record TestDeviceConnectivityCommand(
    Guid DeviceId,
    NetworkProtocol Protocol = NetworkProtocol.Icmp,
    int TimeoutMs = 3000
) : IRequest<DeviceConnectionResultDto>;