using MediatR;
using Nms.Application.Devices.Dtos;

namespace Nms.Application.Discovery.Commands.ImportDiscoveredDevice;

public record ImportDiscoveredDeviceCommand(
    Guid JobId,
    Guid CandidateId,
    string DeviceName) : IRequest<DeviceDto>;