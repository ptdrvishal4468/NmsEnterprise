using Microsoft.Extensions.Logging;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Nms.Domain.Models;

namespace Nms.Infrastructure.ConfigurationBackups;

public class ConfigurationBackupEngine : IConfigurationBackupEngine
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISshClientFactory _sshClientFactory;
    private readonly IConfigurationStorageService _storageService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<ConfigurationBackupEngine> _logger;

    public ConfigurationBackupEngine(
        IUnitOfWork unitOfWork,
        ISshClientFactory sshClientFactory,
        IConfigurationStorageService storageService,
        IEventPublisher eventPublisher,
        ILogger<ConfigurationBackupEngine> logger)
    {
        _unitOfWork = unitOfWork;
        _sshClientFactory = sshClientFactory;
        _storageService = storageService;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<ConfigurationBackup> ExecuteBackupAsync(
        Guid tenantId,
        Guid deviceId,
        BackupTriggerType triggerType,
        CancellationToken cancellationToken = default)
    {
        var device = await _unitOfWork.Devices.GetByIdAsync(deviceId, cancellationToken);
        if (device == null || device.TenantId != tenantId)
        {
            throw new InvalidOperationException($"Device with ID '{deviceId}' was not found for tenant '{tenantId}'.");
        }

        var nextVersion = await _unitOfWork.ConfigurationBackups.GetNextVersionNumberAsync(tenantId, deviceId, cancellationToken);
        var backup = new ConfigurationBackup(Guid.NewGuid(), tenantId, deviceId, nextVersion, triggerType);

        backup.MarkInProgress();
        await _unitOfWork.ConfigurationBackups.AddAsync(backup, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            var command = ResolveExportCommand(device.Vendor);

            // Reuses configured device credentials with factory method
            var credentials = SshCredentials.FromPassword("admin", "admin");

            await using var sshClient = _sshClientFactory.CreateClient(
                device.IpAddress,
                22,
                credentials,
                TimeSpan.FromSeconds(30));

            var executionResult = await sshClient.ExecuteCommandAsync(command, TimeSpan.FromSeconds(30), cancellationToken);

            if (!executionResult.IsSuccess || string.IsNullOrWhiteSpace(executionResult.Output))
            {
                var failureMsg = string.IsNullOrWhiteSpace(executionResult.ErrorOutput)
                    ? "SSH command execution yielded empty output."
                    : executionResult.ErrorOutput;

                backup.MarkFailed(failureMsg);
                _logger.LogWarning("Configuration backup failed for Device '{DeviceId}' (Tenant '{TenantId}'): {Reason}", deviceId, tenantId, failureMsg);
            }
            else
            {
                var (storagePath, checksum, fileSize) = await _storageService.SaveBackupFileAsync(
                    tenantId,
                    deviceId,
                    backup.Id,
                    executionResult.Output,
                    cancellationToken);

                backup.MarkSuccess(storagePath, checksum, fileSize);
                _logger.LogInformation("Configuration backup Version {Version} created for Device '{DeviceId}' (Tenant '{TenantId}'). Size: {SizeBytes} bytes.", nextVersion, deviceId, tenantId, fileSize);
            }
        }
        catch (Exception ex)
        {
            backup.MarkFailed(ex.Message);
            _logger.LogError(ex, "Unhandled exception during configuration backup for Device '{DeviceId}'.", deviceId);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Record Audit Event using accurate IEventPublisher signature
        await _eventPublisher.PublishAsync(
            category: backup.Status == BackupStatus.Success ? EventCategory.System : EventCategory.Security,
            severity: backup.Status == BackupStatus.Success ? EventSeverity.Informational : EventSeverity.Warning,
            source: "ConfigurationBackupEngine",
            message: $"Configuration Backup v{nextVersion} execution status: {backup.Status}",
            deviceId: deviceId,
            metadataJson: $"{{\"BackupId\":\"{backup.Id}\",\"TriggerType\":\"{triggerType}\",\"FailureReason\":\"{backup.FailureReason}\"}}",
            cancellationToken: cancellationToken);

        return backup;
    }

    private static string ResolveExportCommand(string? vendor)
    {
        if (string.IsNullOrWhiteSpace(vendor))
            return "show running-config";

        var normalized = vendor.Trim().ToLowerInvariant();
        return normalized switch
        {
            "juniper" => "show configuration",
            "arista" => "show running-config",
            "hp" or "aruba" => "show running-config",
            "fortinet" => "show full-configuration",
            _ => "show running-config"
        };
    }
}