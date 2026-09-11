using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Health.Dtos;

namespace Nms.Application.Health.Queries.GetDeviceHealthHistory;

public record GetDeviceHealthHistoryQuery(
    Guid DeviceId,
    int PageIndex = 1,
    int PageSize = 20) : IRequest<PagedResult<DeviceHealthHistoryDto>>;