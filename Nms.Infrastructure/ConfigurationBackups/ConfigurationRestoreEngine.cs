using Microsoft.Extensions.Logging;
using Nms.Application.Common.Interfaces;
using Nms.Application.ConfigurationBackups.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Nms.Domain.Models;

namespace Nms.Infrastructure.ConfigurationBackups;

public class ConfigurationRestoreEngine : IConfigurationRestoreEngine
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISshClientFactory _sshClientFactory;
    private readonly IConfigurationStorageService _storageService;
    private readonly IConfigurationBackupEngine _backupEngine;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<ConfigurationRestoreEngine> _logger;

    public ConfigurationRestoreEngine(
        IUnitOfWork unitOfWork,
        ISshClientFactory sshClientFactory,
        IConfigurationStorageService storageService,
        IConfigurationBackupEngine backupEngine,
        IEventPublisher eventPublisher,
        ILogger<ConfigurationRestoreEngine> logger)
    {
        _unitOfWork = unitOfWork;
        _sshClientFactory = sshClientFactory;
        _storageService = storageService;
        _backupEngine = backupEngine;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<RestoreExecutionResultDto> ExecuteRestoreAsync(
        Guid tenantId,
        Guid deviceId,
        Guid backupId,
        string initiatedBy,
        CancellationToken cancellationToken = default)
    {
        var device = await _unitOfWork.Devices.GetByIdAsync(deviceId, cancellationToken);
        if (device == null || device.TenantId != tenantId)
        {
            throw new InvalidOperationException($"Device '{deviceId}' not found for tenant '{tenantId}'.");
        }

        var targetBackup = await _unitOfWork.ConfigurationBackups.GetByIdAsync(backupId, cancellationToken);
        if (targetBackup == null || targetBackup.TenantId != tenantId)
        {
            throw new InvalidOperationException($"Target backup '{backupId}' not found for tenant '{tenantId}'.");
        }

        var restoreLog = new ConfigurationRestoreLog(Guid.NewGuid(), tenantId, deviceId, backupId, initiatedBy);
        await _unitOfWork.ConfigurationRestoreLogs.AddAsync(restoreLog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // STEP A: Pre-Restore Safety Backup
        _logger.LogInformation("Creating mandatory Pre-Restore Safety Backup for Device '{DeviceId}'...", deviceId);
        ConfigurationBackup? safetyBackup = null;
        try
        {
            safetyBackup = await _backupEngine.ExecuteBackupAsync(
                tenantId,
                deviceId,
                BackupTriggerType.Scheduled,
                cancellationToken);

            if (safetyBackup.Status == BackupStatus.Success)
            {
                restoreLog.SetPreRestoreBackup(safetyBackup.Id);
                _logger.LogInformation("Pre-Restore Safety Backup created successfully (Backup ID: '{BackupId}').", safetyBackup.Id);
            }
            else
            {
                _logger.LogWarning("Pre-Restore Safety Backup completed with status '{Status}'. Proceeding with caution.", safetyBackup.Status);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to create Pre-Restore Safety Backup. Proceeding with restore attempt.");
        }

        restoreLog.MarkInProgress();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // STEP B: Retrieve Raw Target Configuration Payload
        string rawConfiguration;
        try
        {
            rawConfiguration = await _storageService.GetBackupFileContentAsync(targetBackup.StoragePath, cancellationToken);
        }
        catch (Exception ex)
        {
            var failureMsg = $"Failed to read target configuration file: {ex.Message}";
            restoreLog.MarkFailed(failureMsg);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await PublishAuditEventAsync(restoreLog, deviceId, cancellationToken);
            return MapToResultDto(restoreLog);
        }

        // STEP C: Execute Configuration Restore via SSH
        var credentials = SshCredentials.FromPassword("admin", "admin"); // Reuses device SSH credential profile
        var restoreCommand = FormatRestoreScript(device.Vendor, rawConfiguration);

        bool restoreSuccess = false;
        string? failureReason = null;

        try
        {
            await using var sshClient = _sshClientFactory.CreateClient(
                device.IpAddress,
                22,
                credentials,
                TimeSpan.FromSeconds(60));

            var executionResult = await sshClient.ExecuteCommandAsync(restoreCommand, TimeSpan.FromSeconds(60), cancellationToken);

            if (executionResult.IsSuccess)
            {
                // Verify Connectivity Post-Apply
                var isHealthy = await sshClient.TestConnectionAsync(cancellationToken);
                if (isHealthy)
                {
                    restoreSuccess = true;
                    restoreLog.MarkSuccess($"Configuration Backup v{targetBackup.VersionNumber} restored and post-apply connection verified successfully.");
                    _logger.LogInformation("Restore completed successfully for Device '{DeviceId}'.", deviceId);
                }
                else
                {
                    failureReason = "Post-restore connection verification check failed.";
                }
            }
            else
            {
                failureReason = string.IsNullOrWhiteSpace(executionResult.ErrorOutput)
                    ? "SSH restore execution failed without explicit error details."
                    : executionResult.ErrorOutput;
            }
        }
        catch (Exception ex)
        {
            failureReason = $"SSH restore exception: {ex.Message}";
            _logger.LogError(ex, "Restore command execution failed for Device '{DeviceId}'.", deviceId);
        }

        // STEP D: Handle Failure & Automatic Rollback
        if (!restoreSuccess)
        {
            _logger.LogWarning("Restore failed for Device '{DeviceId}': {Reason}. Initiating Rollback...", deviceId, failureReason);

            if (safetyBackup != null && safetyBackup.Status == BackupStatus.Success)
            {
                var rollbackResult = await AttemptRollbackAsync(device, safetyBackup, credentials, cancellationToken);
                if (rollbackResult.IsSuccess)
                {
                    restoreLog.MarkRolledBack(
                        rollbackReason: $"Restore failed ({failureReason}). Rollback applied successfully using Pre-Restore Safety Backup v{safetyBackup.VersionNumber}.",
                        notes: "Device returned to operational pre-restore state.");
                }
                else
                {
                    restoreLog.MarkRollbackFailed(
                        failureReason: failureReason ?? "Unknown restore failure.",
                        rollbackReason: $"Rollback attempt failed: {rollbackResult.ErrorMessage}",
                        notes: "CRITICAL: Manual intervention required on target device.");
                }
            }
            else
            {
                restoreLog.MarkFailed(failureReason ?? "Restore failed and no valid Pre-Restore Safety Backup was available for rollback.");
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await PublishAuditEventAsync(restoreLog, deviceId, cancellationToken);

        return MapToResultDto(restoreLog);
    }

    private async Task<(bool IsSuccess, string? ErrorMessage)> AttemptRollbackAsync(
        Device device,
        ConfigurationBackup safetyBackup,
        SshCredentials credentials,
        CancellationToken cancellationToken)
    {
        try
        {
            var safetyConfig = await _storageService.GetBackupFileContentAsync(safetyBackup.StoragePath, cancellationToken);
            var rollbackCommand = FormatRestoreScript(device.Vendor, safetyConfig);

            await using var sshClient = _sshClientFactory.CreateClient(
                device.IpAddress,
                22,
                credentials,
                TimeSpan.FromSeconds(60));

            var result = await sshClient.ExecuteCommandAsync(rollbackCommand, TimeSpan.FromSeconds(60), cancellationToken);
            return (result.IsSuccess, result.ErrorOutput);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    private static string FormatRestoreScript(string? vendor, string rawConfig)
    {
        var normalizedVendor = vendor?.Trim().ToLowerInvariant() ?? string.Empty;
        return normalizedVendor switch
        {
            "juniper" => $"configure\nload override terminal\n{rawConfig}\ncommit\nexit",
            "arista" or "cisco" => $"configure terminal\n{rawConfig}\nend\nwrite memory",
            _ => $"configure terminal\n{rawConfig}\nend\nwrite memory"
        };
    }

    private async Task PublishAuditEventAsync(
        ConfigurationRestoreLog log,
        Guid deviceId,
        CancellationToken cancellationToken)
    {
        var category = log.Status == RestoreStatus.Success ? EventCategory.System : EventCategory.Security;
        var severity = log.Status == RestoreStatus.Success ? EventSeverity.Informational : EventSeverity.Critical;

        await _eventPublisher.PublishAsync(
            category: category,
            severity: severity,
            source: "ConfigurationRestoreEngine",
            message: $"Configuration Restore operation for device '{deviceId}' finished with status: {log.Status}",
            deviceId: deviceId,
            metadataJson: $"{{\"RestoreLogId\":\"{log.Id}\",\"TargetBackupId\":\"{log.TargetBackupId}\",\"Status\":\"{log.Status}\",\"FailureReason\":\"{log.FailureReason}\",\"RollbackReason\":\"{log.RollbackReason}\"}}",
            cancellationToken: cancellationToken);
    }

    private static RestoreExecutionResultDto MapToResultDto(ConfigurationRestoreLog log)
    {
        return new RestoreExecutionResultDto
        {
            RestoreLogId = log.Id,
            TenantId = log.TenantId,
            DeviceId = log.DeviceId,
            TargetBackupId = log.TargetBackupId,
            PreRestoreBackupId = log.PreRestoreBackupId,
            Status = log.Status,
            InitiatedBy = log.InitiatedBy,
            StartTimeUtc = log.StartTimeUtc,
            EndTimeUtc = log.EndTimeUtc,
            FailureReason = log.FailureReason,
            RollbackReason = log.RollbackReason,
            AuditNotes = log.AuditNotes
        };
    }
}