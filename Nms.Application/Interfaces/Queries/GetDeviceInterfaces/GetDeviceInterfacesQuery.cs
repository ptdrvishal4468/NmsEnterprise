using MediatR;
using Nms.Application.Interfaces.Dtos;

namespace Nms.Application.Interfaces.Queries.GetDeviceInterfaces;

public record GetDeviceInterfacesQuery(Guid DeviceId) : IRequest<IReadOnlyList<NetworkInterfaceDto>>;