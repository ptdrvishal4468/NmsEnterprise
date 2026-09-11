using MediatR;
using Microsoft.Extensions.Logging;
using Nms.Application.Common.Interfaces;
using Nms.Application.ConfigurationBackups.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.ConfigurationBackups.Commands.RestoreConfiguration;

public class RestoreConfigurationCommandHandler : IRequestHandler<RestoreConfigurationCommand, RestoreExecutionResultDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly IConfigurationRestoreEngine _restoreEngine;
    private readonly IConfigurationStorageService _storageService;
    private readonly ILogger<RestoreConfigurationCommandHandler> _logger;

    public RestoreConfigurationCommandHandler(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext,
        IConfigurationRestoreEngine restoreEngine,
        IConfigurationStorageService storageService,
        ILogger<RestoreConfigurationCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _restoreEngine = restoreEngine;
        _storageService = storageService;
        _logger = logger;
    }

    public async Task<RestoreExecutionResultDto> Handle(
        RestoreConfigurationCommand request,
        CancellationToken cancellationToken)
    {
        if (_tenantContext.TenantId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Tenant context is required to execute a configuration restore.");
        }

        var tenantId = _tenantContext.TenantId;

        // 1. Fetch Target Backup
        var backup = await _unitOfWork.ConfigurationBackups.GetByIdAsync(request.BackupId, cancellationToken);
        if (backup == null || backup.TenantId != tenantId)
        {
            throw new KeyNotFoundException($"Configuration backup with ID '{request.BackupId}' was not found for current tenant.");
        }

        // 2. Fetch Device
        var device = await _unitOfWork.Devices.GetByIdAsync(backup.DeviceId, cancellationToken);
        if (device == null || device.TenantId != tenantId)
        {
            throw new KeyNotFoundException($"Device with ID '{backup.DeviceId}' associated with backup was not found.");
        }

        // 3. Check Restore Eligibility & Status
        if (backup.Status != BackupStatus.Success || !backup.IsEligibleForRestore)
        {
            throw new InvalidOperationException($"Backup v{backup.VersionNumber} is not eligible for restore. Ensure backup status is 'Success' and restore preparation has passed.");
        }

        // 4. Verify Physical Backup File Integrity on Disk
        try
        {
            var content = await _storageService.GetBackupFileContentAsync(backup.StoragePath, cancellationToken);
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException("Backup configuration file on disk is empty or unreadable.");
            }
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Failed to read backup file from disk at path '{StoragePath}'.", backup.StoragePath);
            throw new InvalidOperationException($"Failed to access physical backup payload: {ex.Message}");
        }

        // 5. Prevent Concurrent Restores on Same Device
        var isBusy = await _unitOfWork.ConfigurationRestoreLogs.HasActiveRestoreInProgressAsync(tenantId, device.Id, cancellationToken);
        if (isBusy)
        {
            throw new InvalidOperationException($"A configuration restore is already in progress for device '{device.Name}' ({device.Id}).");
        }

        const string initiatedBy = "System/Admin";

        _logger.LogInformation("Initiating configuration restore for Device '{DeviceId}' using Backup v{Version} (Initiated by: '{User}').",
            device.Id, backup.VersionNumber, initiatedBy);

        // 6. Delegate Execution to Restore Engine
        return await _restoreEngine.ExecuteRestoreAsync(
            tenantId,
            device.Id,
            backup.Id,
            initiatedBy,
            cancellationToken);
    }
}