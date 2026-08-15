using MediatR;
using Nms.Application.Auditing.Dtos;
using Nms.Application.Common.Models;
using Nms.Domain.Enums;

namespace Nms.Application.Auditing.Queries.SearchAuditLogs;

public record SearchAuditLogsQuery(
    DateTime? FromUtc = null,
    DateTime? ToUtc = null,
    Guid? UserId = null,
    string? Action = null,
    AuditCategory? Category = null,
    AuditStatus? Status = null,
    string? EntityName = null,
    string? EntityId = null,
    string? SearchTerm = null,
    int PageIndex = 1,
    int PageSize = 20
) : IRequest<PagedResult<AuditLogDto>>;