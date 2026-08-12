using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Syslog.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Syslog.Queries.GetSyslogsPaged;

public record GetSyslogsPagedQuery(
    Guid TenantId,
    Guid? DeviceId = null,
    SyslogSeverity? Severity = null,
    SyslogFacility? Facility = null,
    string? SourceIp = null,
    string? SearchKeyword = null,
    DateTime? StartDateUtc = null,
    DateTime? EndDateUtc = null,
    int PageNumber = 1,
    int PageSize = 50) : IRequest<PagedResult<SyslogMessageDto>>;