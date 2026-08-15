using MediatR;
using Nms.Domain.Enums;

namespace Nms.Application.Auditing.Commands.RecordAuditLog;

public record RecordAuditLogCommand(
    Guid? UserId,
    string? Username,
    string Action,
    string? EntityName = null,
    string? EntityId = null,
    AuditCategory Category = AuditCategory.System,
    AuditStatus Status = AuditStatus.Success,
    string? IpAddress = null,
    string? Details = null,
    string? OldValuesJson = null,
    string? NewValuesJson = null
) : IRequest<long>;