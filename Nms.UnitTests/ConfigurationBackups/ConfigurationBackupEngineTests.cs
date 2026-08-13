using Microsoft.Extensions.Logging;
using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Nms.Domain.Models;
using Nms.Infrastructure.ConfigurationBackups;

namespace Nms.UnitTests.ConfigurationBackups;

public class ConfigurationBackupEngineTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IDeviceRepository> _deviceRepositoryMock = new();
    private readonly Mock<IConfigurationBackupRepository> _backupRepositoryMock = new();
    private readonly Mock<ISshClientFactory> _sshClientFactoryMock = new();
    private readonly Mock<ISshClient> _sshClientMock = new();
    private readonly Mock<IConfigurationStorageService> _storageServiceMock = new();
    private readonly Mock<IEventPublisher> _eventPublisherMock = new();
    private readonly Mock<ILogger<ConfigurationBackupEngine>> _loggerMock = new();

    public ConfigurationBackupEngineTests()
    {
        _unitOfWorkMock.Setup(u => u.Devices).Returns(_deviceRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.ConfigurationBackups).Returns(_backupRepositoryMock.Object);

        _sshClientFactoryMock
            .Setup(f => f.CreateClient(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<SshCredentials>(), It.IsAny<TimeSpan>()))
            .Returns(_sshClientMock.Object);
    }

    [Fact]
    public async Task ExecuteBackupAsync_DeviceNotFound_ThrowsInvalidOperationException()
    {
        var tenantId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();

        _deviceRepositoryMock
            .Setup(r => r.GetByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Device?)null);

        var engine = new ConfigurationBackupEngine(
            _unitOfWorkMock.Object,
            _sshClientFactoryMock.Object,
            _storageServiceMock.Object,
            _eventPublisherMock.Object,
            _loggerMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            engine.ExecuteBackupAsync(tenantId, deviceId, BackupTriggerType.Manual, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteBackupAsync_ValidDevice_ExecutesSshBackupAndSavesFile()
    {
        var tenantId = Guid.NewGuid();
        var device = new Device(Guid.NewGuid(), tenantId, "Cisco-Core-R1", "10.0.0.1", DeviceType.Router, vendor: "cisco");

        _deviceRepositoryMock
            .Setup(r => r.GetByIdAsync(device.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(device);

        _backupRepositoryMock
            .Setup(r => r.GetNextVersionNumberAsync(tenantId, device.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _sshClientMock
            .Setup(c => c.ExecuteCommandAsync(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(SshExecutionResult.Success("hostname Cisco-Core-R1\ninterface GigabitEthernet0/0", 120));

        _storageServiceMock
            .Setup(s => s.SaveBackupFileAsync(tenantId, device.Id, It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(("App_Data/Backups/file.config", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", 1024));

        var engine = new ConfigurationBackupEngine(
            _unitOfWorkMock.Object,
            _sshClientFactoryMock.Object,
            _storageServiceMock.Object,
            _eventPublisherMock.Object,
            _loggerMock.Object);

        var result = await engine.ExecuteBackupAsync(tenantId, device.Id, BackupTriggerType.Manual, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(BackupStatus.Success, result.Status);
        Assert.Equal(1, result.VersionNumber);
        Assert.Equal(1024, result.FileSizeBytes);

        _storageServiceMock.Verify(s => s.SaveBackupFileAsync(
            tenantId, device.Id, result.Id, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}