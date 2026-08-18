using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Cybersecurity.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Cybersecurity.Queries.GetDeviceComplianceScansPaged;

public record GetDeviceComplianceScansPagedQuery(
    int PageIndex = 1,
    int PageSize = 20,
    Guid? DeviceId = null,
    ComplianceStatus? OverallStatus = null,
    DateTime? FromDateUtc = null,
    DateTime? ToDateUtc = null) : IRequest<PagedResult<DeviceComplianceScanDto>>;