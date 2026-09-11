using MediatR;

namespace Nms.Application.Devices.Commands.DeleteDevice;

public record DeleteDeviceCommand(Guid Id) : IRequest<bool>;