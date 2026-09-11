using MediatR;
using Nms.Application.Health.Dtos;

namespace Nms.Application.Health.Queries.GetDeviceHealth;

public record GetDeviceHealthQuery(Guid DeviceId) : IRequest<DeviceHealthDto>;