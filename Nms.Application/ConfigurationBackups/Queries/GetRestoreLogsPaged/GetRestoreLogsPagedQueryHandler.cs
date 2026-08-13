using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Application.ConfigurationBackups.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.ConfigurationBackups.Queries.GetRestoreLogsPaged;

public class GetRestoreLogsPagedQueryHandler : IRequestHandler<GetRestoreLogsPagedQuery, PagedResult<ConfigurationRestoreLogDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetRestoreLogsPagedQueryHandler(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<PagedResult<ConfigurationRestoreLogDto>> Handle(
        GetRestoreLogsPagedQuery request,
        CancellationToken cancellationToken)
    {
        if (_tenantContext.TenantId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Tenant context is required.");
        }

        var tenantId = _tenantContext.TenantId;

        var logs = request.DeviceId.HasValue
            ? await _unitOfWork.ConfigurationRestoreLogs.GetByDeviceIdAsync(tenantId, request.DeviceId.Value, cancellationToken)
            : await _unitOfWork.ConfigurationRestoreLogs.GetAllAsync(cancellationToken);

        var tenantFilteredLogs = logs
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.StartTimeUtc)
            .ToList();

        var totalCount = tenantFilteredLogs.Count;
        var pagedItems = tenantFilteredLogs
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ConfigurationRestoreLogDto
            {
                Id = x.Id,
                TenantId = x.TenantId,
                DeviceId = x.DeviceId,
                DeviceName = x.Device?.Name ?? string.Empty,
                TargetBackupId = x.TargetBackupId,
                TargetVersionNumber = x.TargetBackup?.VersionNumber ?? 0,
                PreRestoreBackupId = x.PreRestoreBackupId,
                Status = x.Status,
                InitiatedBy = x.InitiatedBy,
                StartTimeUtc = x.StartTimeUtc,
                EndTimeUtc = x.EndTimeUtc,
                FailureReason = x.FailureReason,
                RollbackReason = x.RollbackReason,
                AuditNotes = x.AuditNotes,
                CreatedAtUtc = x.CreatedAtUtc
            })
            .ToList();

        return new PagedResult<ConfigurationRestoreLogDto>(pagedItems, totalCount, request.PageNumber, request.PageSize);
    }
}