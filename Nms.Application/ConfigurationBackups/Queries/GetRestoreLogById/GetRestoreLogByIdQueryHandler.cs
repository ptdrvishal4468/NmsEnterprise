using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.ConfigurationBackups.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.ConfigurationBackups.Queries.GetRestoreLogById;

public class GetRestoreLogByIdQueryHandler : IRequestHandler<GetRestoreLogByIdQuery, ConfigurationRestoreLogDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetRestoreLogByIdQueryHandler(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<ConfigurationRestoreLogDto?> Handle(
        GetRestoreLogByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (_tenantContext.TenantId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Tenant context is required.");
        }

        var tenantId = _tenantContext.TenantId;
        var log = await _unitOfWork.ConfigurationRestoreLogs.GetByIdAsync(request.RestoreLogId, cancellationToken);

        if (log == null || log.TenantId != tenantId)
        {
            return null;
        }

        return new ConfigurationRestoreLogDto
        {
            Id = log.Id,
            TenantId = log.TenantId,
            DeviceId = log.DeviceId,
            DeviceName = log.Device?.Name ?? string.Empty,
            TargetBackupId = log.TargetBackupId,
            TargetVersionNumber = log.TargetBackup?.VersionNumber ?? 0,
            PreRestoreBackupId = log.PreRestoreBackupId,
            Status = log.Status,
            InitiatedBy = log.InitiatedBy,
            StartTimeUtc = log.StartTimeUtc,
            EndTimeUtc = log.EndTimeUtc,
            FailureReason = log.FailureReason,
            RollbackReason = log.RollbackReason,
            AuditNotes = log.AuditNotes,
            CreatedAtUtc = log.CreatedAtUtc
        };
    }
}