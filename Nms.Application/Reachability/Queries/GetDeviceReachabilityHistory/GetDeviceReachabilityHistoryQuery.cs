using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Reachability.Dtos;

namespace Nms.Application.Reachability.Queries.GetDeviceReachabilityHistory;

public sealed record GetDeviceReachabilityHistoryQuery(
    Guid DeviceId,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PagedResult<ReachabilityHistoryDto>>;