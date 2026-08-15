using MediatR;
using Nms.Application.Auditing.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Auditing.Queries.GetAuditLogById;

public class GetAuditLogByIdQueryHandler : IRequestHandler<GetAuditLogByIdQuery, AuditLogDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAuditLogByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AuditLogDto?> Handle(GetAuditLogByIdQuery request, CancellationToken cancellationToken)
    {
        var auditLog = await _unitOfWork.AuditLogs.GetByIdAsync(request.Id, cancellationToken);
        if (auditLog == null) return null;

        return new AuditLogDto
        {
            Id = auditLog.Id,
            TenantId = auditLog.TenantId,
            UserId = auditLog.UserId,
            Username = auditLog.Username,
            Action = auditLog.Action,
            EntityName = auditLog.EntityName,
            EntityId = auditLog.EntityId,
            Category = auditLog.Category.ToString(),
            Status = auditLog.Status.ToString(),
            IpAddress = auditLog.IpAddress,
            Details = auditLog.Details,
            OldValuesJson = auditLog.OldValuesJson,
            NewValuesJson = auditLog.NewValuesJson,
            TimestampUtc = auditLog.TimestampUtc
        };
    }
}