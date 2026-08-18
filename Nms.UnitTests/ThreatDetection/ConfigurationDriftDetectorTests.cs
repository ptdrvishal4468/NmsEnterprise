using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.ThreatDetection.Services;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.ThreatDetection;

public class ConfigurationDriftDetectorTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IConfigurationBackupRepository> _mockBackupRepo;
    private readonly Mock<IConfigurationDriftRepository> _mockDriftRepo;
    private readonly Mock<IThreatIndicatorRepository> _mockThreatRepo;
    private readonly Mock<IConfigurationStorageService> _mockStorageService;
    private readonly Mock<ITenantContext> _mockTenantContext;
    private readonly Guid _tenantId = Guid.NewGuid();
    private readonly Guid _deviceId = Guid.NewGuid();

    public ConfigurationDriftDetectorTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockBackupRepo = new Mock<IConfigurationBackupRepository>();
        _mockDriftRepo = new Mock<IConfigurationDriftRepository>();
        _mockThreatRepo = new Mock<IThreatIndicatorRepository>();
        _mockStorageService = new Mock<IConfigurationStorageService>();
        _mockTenantContext = new Mock<ITenantContext>();

        _mockTenantContext.Setup(t => t.TenantId).Returns(_tenantId);
        _mockUnitOfWork.Setup(u => u.ConfigurationBackups).Returns(_mockBackupRepo.Object);
        _mockUnitOfWork.Setup(u => u.ConfigurationDrifts).Returns(_mockDriftRepo.Object);
        _mockUnitOfWork.Setup(u => u.ThreatIndicators).Returns(_mockThreatRepo.Object);
        _mockUnitOfWork.Setup(u => u.Devices).Returns(new Mock<IDeviceRepository>().Object);
    }

    [Fact]
    public async Task AnalyzeDeviceDriftAsync_ShouldDetectDrift_WhenRunningDiffersFromBaseline()
    {
        var backup1Id = Guid.NewGuid();
        var backup2Id = Guid.NewGuid();

        var backup1 = new ConfigurationBackup(backup1Id, _tenantId, _deviceId, 1, BackupTriggerType.Manual);
        backup1.MarkSuccess("/backups/1.cfg", "hash1", 100);

        var backup2 = new ConfigurationBackup(backup2Id, _tenantId, _deviceId, 2, BackupTriggerType.Manual);
        backup2.MarkSuccess("/backups/2.cfg", "hash2", 120);

        var backups = new List<ConfigurationBackup> { backup1, backup2 };

        _mockBackupRepo
            .Setup(u => u.GetBackupsPagedAsync(
                _tenantId,
                _deviceId,
                BackupStatus.Success,
                It.IsAny<BackupTriggerType?>(),
                1,
                50,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((backups, backups.Count));

        _mockStorageService
            .Setup(s => s.GetBackupFileContentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string path, CancellationToken _) =>
            {
                if (path == "/backups/1.cfg")
                    return "hostname CoreSwitch\ninterface GigabitEthernet0/1\n no shutdown";
                if (path == "/backups/2.cfg")
                    return "hostname CoreSwitch\ninterface GigabitEthernet0/1\n shutdown\ninterface GigabitEthernet0/2\n no shutdown";
                return string.Empty;
            });

        _mockDriftRepo
            .Setup(d => d.AddAsync(It.IsAny<ConfigurationDriftRecord>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockThreatRepo
            .Setup(u => u.GetActiveIndicatorAsync(
                _tenantId,
                ThreatType.ConfigurationDrift,
                null,
                _deviceId,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ThreatIndicator?)null);

        _mockThreatRepo
            .Setup(u => u.AddAsync(It.IsAny<ThreatIndicator>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var detector = new ConfigurationDriftDetector(_mockUnitOfWork.Object, _mockStorageService.Object, _mockTenantContext.Object);

        var result = await detector.AnalyzeDeviceDriftAsync(_deviceId);

        Assert.NotNull(result);
        Assert.True(result.HasDrift);
        Assert.True(result.AddedLinesCount > 0);
        Assert.True(result.RemovedLinesCount >= 0);
        _mockDriftRepo.Verify(u => u.AddAsync(It.IsAny<ConfigurationDriftRecord>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}