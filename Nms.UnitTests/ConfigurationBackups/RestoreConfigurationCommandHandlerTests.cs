using Microsoft.Extensions.Logging;
using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.ConfigurationBackups.Commands.RestoreConfiguration;
using Nms.Application.ConfigurationBackups.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.ConfigurationBackups;

public class RestoreConfigurationCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly Mock<IConfigurationRestoreEngine> _restoreEngineMock;
    private readonly Mock<IConfigurationStorageService> _storageServiceMock;
    private readonly Mock<ILogger<RestoreConfigurationCommandHandler>> _loggerMock;
    private readonly RestoreConfigurationCommandHandler _handler;

    public RestoreConfigurationCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tenantContextMock = new Mock<ITenantContext>();
        _restoreEngineMock = new Mock<IConfigurationRestoreEngine>();
        _storageServiceMock = new Mock<IConfigurationStorageService>();
        _loggerMock = new Mock<ILogger<RestoreConfigurationCommandHandler>>();

        _handler = new RestoreConfigurationCommandHandler(
            _unitOfWorkMock.Object,
            _tenantContextMock.Object,
            _restoreEngineMock.Object,
            _storageServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenTenantContextNotResolved()
    {
        // Arrange
        _tenantContextMock.Setup(t => t.TenantId).Returns(Guid.Empty);
        var command = new RestoreConfigurationCommand(Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenBackupDoesNotExist()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var backupId = Guid.NewGuid();

        _tenantContextMock.Setup(t => t.TenantId).Returns(tenantId);
        _unitOfWorkMock.Setup(u => u.ConfigurationBackups.GetByIdAsync(backupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConfigurationBackup?)null);

        var command = new RestoreConfigurationCommand(backupId);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidOperationException_WhenBackupIsNotEligibleForRestore()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var backup = new ConfigurationBackup(Guid.NewGuid(), tenantId, deviceId, 1, BackupTriggerType.Manual);

        // Backup is marked successful but IsEligibleForRestore remains false
        backup.MarkSuccess("App_Data/Backups/file.config", "sha256hash", 1024);

        _tenantContextMock.Setup(t => t.TenantId).Returns(tenantId);
        _unitOfWorkMock.Setup(u => u.ConfigurationBackups.GetByIdAsync(backup.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(backup);

        var device = new Device(deviceId, tenantId, "Core-Switch-01", "192.168.1.10", DeviceType.Switch);
        _unitOfWorkMock.Setup(u => u.Devices.GetByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(device);

        var command = new RestoreConfigurationCommand(backup.Id);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Contains("not eligible for restore", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidOperationException_WhenActiveRestoreIsAlreadyInProgress()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var backup = new ConfigurationBackup(Guid.NewGuid(), tenantId, deviceId, 1, BackupTriggerType.Manual);

        backup.MarkSuccess("App_Data/Backups/file.config", "sha256hash", 1024);
        backup.SetRestorePreparation(true, "Verified");

        _tenantContextMock.Setup(t => t.TenantId).Returns(tenantId);
        _unitOfWorkMock.Setup(u => u.ConfigurationBackups.GetByIdAsync(backup.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(backup);

        var device = new Device(deviceId, tenantId, "Core-Switch-01", "192.168.1.10", DeviceType.Switch);
        _unitOfWorkMock.Setup(u => u.Devices.GetByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(device);

        _storageServiceMock.Setup(s => s.GetBackupFileContentAsync(backup.StoragePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync("hostname Core-Switch-01");

        _unitOfWorkMock.Setup(u => u.ConfigurationRestoreLogs.HasActiveRestoreInProgressAsync(tenantId, deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); // Active restore lock exists

        var command = new RestoreConfigurationCommand(backup.Id);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Contains("already in progress", ex.Message);
    }

    [Fact]
    public async Task Handle_ShouldDelegateToRestoreEngine_WhenAllValidationsPass()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var backup = new ConfigurationBackup(Guid.NewGuid(), tenantId, deviceId, 1, BackupTriggerType.Manual);

        backup.MarkSuccess("App_Data/Backups/file.config", "sha256hash", 1024);
        backup.SetRestorePreparation(true, "Verified");

        _tenantContextMock.Setup(t => t.TenantId).Returns(tenantId);
        _unitOfWorkMock.Setup(u => u.ConfigurationBackups.GetByIdAsync(backup.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(backup);

        var device = new Device(deviceId, tenantId, "Core-Switch-01", "192.168.1.10", DeviceType.Switch);
        _unitOfWorkMock.Setup(u => u.Devices.GetByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(device);

        _storageServiceMock.Setup(s => s.GetBackupFileContentAsync(backup.StoragePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync("hostname Core-Switch-01");

        _unitOfWorkMock.Setup(u => u.ConfigurationRestoreLogs.HasActiveRestoreInProgressAsync(tenantId, deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var expectedResult = new RestoreExecutionResultDto
        {
            RestoreLogId = Guid.NewGuid(),
            TenantId = tenantId,
            DeviceId = deviceId,
            TargetBackupId = backup.Id,
            Status = RestoreStatus.Success
        };

        _restoreEngineMock.Setup(e => e.ExecuteRestoreAsync(tenantId, deviceId, backup.Id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var command = new RestoreConfigurationCommand(backup.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(RestoreStatus.Success, result.Status);
        _restoreEngineMock.Verify(e => e.ExecuteRestoreAsync(tenantId, deviceId, backup.Id, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}