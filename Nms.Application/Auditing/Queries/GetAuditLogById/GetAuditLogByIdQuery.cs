using MediatR;
using Nms.Application.Auditing.Dtos;

namespace Nms.Application.Auditing.Queries.GetAuditLogById;

public record GetAuditLogByIdQuery(long Id) : IRequest<AuditLogDto?>;