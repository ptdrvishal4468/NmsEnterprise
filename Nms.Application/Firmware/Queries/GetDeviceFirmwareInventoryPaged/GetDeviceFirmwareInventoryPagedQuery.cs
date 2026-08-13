using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Firmware.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Firmware.Queries.GetDeviceFirmwareInventoryPaged;

public record GetDeviceFirmwareInventoryPagedQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    FirmwareComplianceStatus? ComplianceStatus = null) : IRequest<PagedResult<DeviceFirmwareInventoryDto>>;