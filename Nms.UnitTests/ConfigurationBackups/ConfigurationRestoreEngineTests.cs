using Microsoft.Extensions.Logging;
using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Nms.Domain.Models;
using Nms.Infrastructure.ConfigurationBackups;
using Xunit;

namespace Nms.UnitTests.ConfigurationBackups;

public class ConfigurationRestoreEngineTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ISshClientFactory> _sshClientFactoryMock;
    private readonly Mock<IConfigurationStorageService> _storageServiceMock;
    private readonly Mock<IConfigurationBackupEngine> _backupEngineMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly Mock<ILogger<ConfigurationRestoreEngine>> _loggerMock;
    private readonly ConfigurationRestoreEngine _engine;

    public ConfigurationRestoreEngineTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sshClientFactoryMock = new Mock<ISshClientFactory>();
        _storageServiceMock = new Mock<IConfigurationStorageService>();
        _backupEngineMock = new Mock<IConfigurationBackupEngine>();
        _eventPublisherMock = new Mock<IEventPublisher>();
        _loggerMock = new Mock<ILogger<ConfigurationRestoreEngine>>();

        _engine = new ConfigurationRestoreEngine(
            _unitOfWorkMock.Object,
            _sshClientFactoryMock.Object,
            _storageServiceMock.Object,
            _backupEngineMock.Object,
            _eventPublisherMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteRestoreAsync_ShouldExecuteRestoreSuccessfully_WhenSshAndVerificationSucceed()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var targetBackupId = Guid.NewGuid();

        var device = new Device(deviceId, tenantId, "Edge-Router-01", "10.0.0.1", DeviceType.Router);
        _unitOfWorkMock.Setup(u => u.Devices.GetByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(device);

        var targetBackup = new ConfigurationBackup(targetBackupId, tenantId, deviceId, 2, BackupTriggerType.Manual);
        targetBackup.MarkSuccess("App_Data/Backups/v2.config", "hash2", 2048);

        _unitOfWorkMock.Setup(u => u.ConfigurationBackups.GetByIdAsync(targetBackupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetBackup);

        // Pre-Restore Safety Backup Mock
        var safetyBackup = new ConfigurationBackup(Guid.NewGuid(), tenantId, deviceId, 3, BackupTriggerType.Scheduled);
        safetyBackup.MarkSuccess("App_Data/Backups/v3_safety.config", "hash3", 2050);

        _backupEngineMock.Setup(b => b.ExecuteBackupAsync(tenantId, deviceId, BackupTriggerType.Scheduled, It.IsAny<CancellationToken>()))
            .ReturnsAsync(safetyBackup);

        _unitOfWorkMock.Setup(u => u.ConfigurationRestoreLogs.AddAsync(It.IsAny<ConfigurationRestoreLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _storageServiceMock.Setup(s => s.GetBackupFileContentAsync(targetBackup.StoragePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync("hostname Edge-Router-01\ninterface GigabitEthernet0/0");

        // Mock SSH Client
        var sshClientMock = new Mock<ISshClient>();
        sshClientMock.Setup(s => s.ExecuteCommandAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(SshExecutionResult.Success("Configuration applied successfully", 120, 0));

        sshClientMock.Setup(s => s.TestConnectionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _sshClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<SshCredentials>(), It.IsAny<TimeSpan>()))
            .Returns(sshClientMock.Object);

        // Act
        var result = await _engine.ExecuteRestoreAsync(tenantId, deviceId, targetBackupId, "AdminUser", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(RestoreStatus.Success, result.Status);
        Assert.Equal(safetyBackup.Id, result.PreRestoreBackupId);

        // Positional parameter matching for Expression Tree inside Moq .Verify()
        _eventPublisherMock.Verify(e => e.PublishAsync(
            It.IsAny<EventCategory>(),
            It.IsAny<EventSeverity>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<Guid?>(),
            It.IsAny<string>(),
            It.IsAny<Guid?>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteRestoreAsync_ShouldTriggerRollback_WhenSshRestoreExecutionFails()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var targetBackupId = Guid.NewGuid();

        var device = new Device(deviceId, tenantId, "Edge-Router-01", "10.0.0.1", DeviceType.Router);
        _unitOfWorkMock.Setup(u => u.Devices.GetByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(device);

        var targetBackup = new ConfigurationBackup(targetBackupId, tenantId, deviceId, 1, BackupTriggerType.Manual);
        targetBackup.MarkSuccess("App_Data/Backups/v1.config", "hash1", 1000);

        _unitOfWorkMock.Setup(u => u.ConfigurationBackups.GetByIdAsync(targetBackupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetBackup);

        // Pre-Restore Safety Backup Mock
        var safetyBackup = new ConfigurationBackup(Guid.NewGuid(), tenantId, deviceId, 2, BackupTriggerType.Scheduled);
        safetyBackup.MarkSuccess("App_Data/Backups/v2_safety.config", "hash2", 1050);

        _backupEngineMock.Setup(b => b.ExecuteBackupAsync(tenantId, deviceId, BackupTriggerType.Scheduled, It.IsAny<CancellationToken>()))
            .ReturnsAsync(safetyBackup);

        _unitOfWorkMock.Setup(u => u.ConfigurationRestoreLogs.AddAsync(It.IsAny<ConfigurationRestoreLog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _storageServiceMock.Setup(s => s.GetBackupFileContentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("raw_config_payload");

        // Mock SSH Client to fail restore, but succeed on rollback
        var sshClientMock = new Mock<ISshClient>();

        // 1st call (Restore) fails; 2nd call (Rollback) succeeds
        sshClientMock.SetupSequence(s => s.ExecuteCommandAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(SshExecutionResult.Failure("Syntax error in configuration script", 50, -1))
            .ReturnsAsync(SshExecutionResult.Success("Rollback configuration restored successfully", 80, 0));

        _sshClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<SshCredentials>(), It.IsAny<TimeSpan>()))
            .Returns(sshClientMock.Object);

        // Act
        var result = await _engine.ExecuteRestoreAsync(tenantId, deviceId, targetBackupId, "AdminUser", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal(RestoreStatus.RolledBack, result.Status);
        Assert.NotNull(result.RollbackReason);
        Assert.Contains("Syntax error", result.RollbackReason);
    }
}