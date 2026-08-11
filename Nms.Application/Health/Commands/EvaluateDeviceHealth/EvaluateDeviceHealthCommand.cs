using MediatR;
using Nms.Application.Health.Dtos;

namespace Nms.Application.Health.Commands.EvaluateDeviceHealth;

public record EvaluateDeviceHealthCommand(Guid DeviceId) : IRequest<DeviceHealthDto>;