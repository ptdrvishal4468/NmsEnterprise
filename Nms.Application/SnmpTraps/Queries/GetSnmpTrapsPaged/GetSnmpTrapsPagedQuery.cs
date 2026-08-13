using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.SnmpTraps.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.SnmpTraps.Queries.GetSnmpTrapsPaged;

public sealed record GetSnmpTrapsPagedQuery(
    Guid TenantId,
    Guid? DeviceId = null,
    TrapSeverity? Severity = null,
    string? SourceIpAddress = null,
    string? EnterpriseOid = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<PagedResult<SnmpTrapMessageDto>>;