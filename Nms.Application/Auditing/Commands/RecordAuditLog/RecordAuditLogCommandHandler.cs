using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Auditing.Commands.RecordAuditLog;

public class RecordAuditLogCommandHandler : IRequestHandler<RecordAuditLogCommand, long>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public RecordAuditLogCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<long> Handle(RecordAuditLogCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var userId = request.UserId ?? Guid.Empty;

        var auditLog = new AuditLog(
            tenantId: tenantId,
            userId: userId,
            action: request.Action,
            oldValuesJson: request.OldValuesJson,
            newValuesJson: request.NewValuesJson,
            username: request.Username,
            entityName: request.EntityName,
            entityId: request.EntityId,
            category: request.Category,
            status: request.Status,
            ipAddress: request.IpAddress,
            details: request.Details);

        await _unitOfWork.AuditLogs.AddAsync(auditLog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return auditLog.Id;
    }
}