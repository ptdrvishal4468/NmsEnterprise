using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Devices.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Devices.Queries.GetDevicesPaged;

public record GetDevicesPagedQuery(
    int PageIndex = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    DeviceType? DeviceType = null,
    DeviceStatus? Status = null
) : IRequest<PagedResult<DeviceDto>>;