using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.ConfigurationBackups.Commands.PrepareRestore;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.UnitTests.ConfigurationBackups;

public class PrepareRestoreCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IConfigurationBackupRepository> _backupRepositoryMock = new();
    private readonly Mock<IConfigurationStorageService> _storageServiceMock = new();

    public PrepareRestoreCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.ConfigurationBackups).Returns(_backupRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_BackupNotFound_ThrowsKeyNotFoundException()
    {
        var tenantId = Guid.NewGuid();
        var backupId = Guid.NewGuid();

        _backupRepositoryMock
            .Setup(r => r.GetByIdAsync(backupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConfigurationBackup?)null);

        var handler = new PrepareRestoreCommandHandler(_unitOfWorkMock.Object, _storageServiceMock.Object);
        var command = new PrepareRestoreCommand(tenantId, backupId);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ValidBackup_PreparesRestoreSuccessfully()
    {
        var tenantId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var backup = new ConfigurationBackup(Guid.NewGuid(), tenantId, deviceId, 1, BackupTriggerType.Manual);
        backup.MarkSuccess("App_Data/Backups/test.config", "dummyhash", 2048);

        _backupRepositoryMock
            .Setup(r => r.GetByIdAsync(backup.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(backup);

        _storageServiceMock
            .Setup(s => s.GetBackupFileContentAsync(backup.StoragePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync("hostname Cisco-Core-R1");

        var handler = new PrepareRestoreCommandHandler(_unitOfWorkMock.Object, _storageServiceMock.Object);
        var command = new PrepareRestoreCommand(tenantId, backup.Id, "Ready for restore test.");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsEligibleForRestore);
        Assert.True(result.FileExistsOnDisk);
        Assert.Equal(backup.Id, result.BackupId);
    }
}