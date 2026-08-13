using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Firmware.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Firmware.Queries.GetUpgradePlansPaged;

public record GetUpgradePlansPagedQuery(
    Guid? DeviceId = null,
    UpgradePlanStatus? Status = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<PagedResult<FirmwareUpgradePlanDto>>;