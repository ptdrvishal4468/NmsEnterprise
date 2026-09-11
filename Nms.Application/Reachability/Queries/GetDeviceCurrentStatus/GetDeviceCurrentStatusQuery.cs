using MediatR;
using Nms.Application.Reachability.Dtos;

namespace Nms.Application.Reachability.Queries.GetDeviceCurrentStatus;

public sealed record GetDeviceCurrentStatusQuery(Guid DeviceId) : IRequest<PingSummaryDto>;