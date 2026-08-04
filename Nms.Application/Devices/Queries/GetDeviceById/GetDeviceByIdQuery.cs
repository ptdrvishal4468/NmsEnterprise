using MediatR;
using Nms.Application.Devices.Dtos;

namespace Nms.Application.Devices.Queries.GetDeviceById;

public record GetDeviceByIdQuery(Guid Id) : IRequest<DeviceDto?>;