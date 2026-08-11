using MediatR;
using Nms.Application.Interfaces.Dtos;

namespace Nms.Application.Interfaces.Commands.PollDeviceInterfaces;

public record PollDeviceInterfacesCommand(Guid DeviceId) : IRequest<IReadOnlyList<NetworkInterfaceDto>>;