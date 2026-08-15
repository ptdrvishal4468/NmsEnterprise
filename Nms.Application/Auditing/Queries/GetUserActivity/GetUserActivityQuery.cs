using MediatR;
using Nms.Application.Auditing.Dtos;
using Nms.Application.Common.Models;

namespace Nms.Application.Auditing.Queries.GetUserActivity;

public record GetUserActivityQuery(
    Guid? UserId = null,
    DateTime? FromUtc = null,
    DateTime? ToUtc = null,
    int PageIndex = 1,
    int PageSize = 20
) : IRequest<PagedResult<AuditLogDto>>;