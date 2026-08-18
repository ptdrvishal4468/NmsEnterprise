using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Firmware.Services;
using Nms.Application.Vulnerabilities.Services;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Vulnerabilities;

public class FirmwareUpgradeRecommendationEngineTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IFirmwareBaselineRepository> _mockBaselineRepo;
    private readonly Mock<IFirmwareUpgradeRecommendationRepository> _mockRecRepo;
    private readonly IFirmwareVersionComparator _versionComparator;
    private readonly FirmwareUpgradeRecommendationEngine _engine;

    public FirmwareUpgradeRecommendationEngineTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockBaselineRepo = new Mock<IFirmwareBaselineRepository>();
        _mockRecRepo = new Mock<IFirmwareUpgradeRecommendationRepository>();
        _versionComparator = new FirmwareVersionComparator();

        _mockUow.Setup(u => u.FirmwareBaselines).Returns(_mockBaselineRepo.Object);
        _mockUow.Setup(u => u.FirmwareUpgradeRecommendations).Returns(_mockRecRepo.Object);

        _engine = new FirmwareUpgradeRecommendationEngine(_mockUow.Object, _versionComparator);
    }

    [Fact]
    public async Task GenerateRecommendation_WithBaseline_PrioritizesBaselineVersion()
    {
        var tenantId = Guid.NewGuid();
        var device = new Device(
            Guid.NewGuid(),
            tenantId,
            "Edge-Switch",
            "192.168.1.1",
            DeviceType.Switch,
            vendor: "Cisco",
            model: "Catalyst 2960",
            firmwareVersion: "15.0.2");

        var vuln = new Vulnerability(
            Guid.NewGuid(),
            tenantId,
            "CVE-2026-1001",
            "IOS Flaw",
            "Desc",
            VulnerabilitySeverity.Critical,
            9.8m,
            "Cisco",
            null,
            "15.0.0",
            "15.2.0",
            "15.2.4");

        var match = new DeviceVulnerabilityMatch(Guid.NewGuid(), tenantId, device.Id, vuln.Id, "15.0.2", DateTime.UtcNow);
        typeof(DeviceVulnerabilityMatch).GetProperty(nameof(DeviceVulnerabilityMatch.Vulnerability))!.SetValue(match, vuln);

        var baseline = new FirmwareBaseline(Guid.NewGuid(), tenantId, "Cisco", "Catalyst 2960", "15.4.1");
        _mockBaselineRepo.Setup(r => r.GetByVendorAndModelAsync(tenantId, "Cisco", "Catalyst 2960", It.IsAny<CancellationToken>()))
            .ReturnsAsync(baseline);

        _mockRecRepo.Setup(r => r.GetActiveByDeviceIdAsync(tenantId, device.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FirmwareUpgradeRecommendation>());

        var result = await _engine.GenerateRecommendationForDeviceAsync(tenantId, device, new List<DeviceVulnerabilityMatch> { match });

        Assert.NotNull(result);
        Assert.Equal("15.4.1", result.RecommendedVersion);
        Assert.Equal(RecommendationPriority.Critical, result.Priority);
        Assert.Equal(1, result.CriticalVulnerabilityCount);
    }
}