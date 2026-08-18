using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Vulnerabilities.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Vulnerabilities.Queries.GetDeviceVulnerabilitiesPaged;

public record GetDeviceVulnerabilitiesPagedQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? DeviceId = null,
    VulnerabilityStatus? Status = null,
    VulnerabilitySeverity? Severity = null) : IRequest<PagedResult<DeviceVulnerabilityMatchDto>>;