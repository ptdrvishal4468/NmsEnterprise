using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.ThreatDetection.Services;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.ThreatDetection;

public class PortScanDetectorTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ITenantContext> _mockTenantContext;
    private readonly Guid _tenantId = Guid.NewGuid();

    public PortScanDetectorTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTenantContext = new Mock<ITenantContext>();
        _mockTenantContext.Setup(t => t.TenantId).Returns(_tenantId);
    }

    [Fact]
    public async Task DetectPortScanIndicatorsAsync_ShouldFlagHost_WhenScanLogsExceedThreshold()
    {
        // Arrange
        var syslogs = new List<SyslogMessage>
        {
            new() { TenantId = _tenantId, Hostname = "scanner.local", Message = "TCP connection denied on port 22", TimestampUtc = DateTime.UtcNow },
            new() { TenantId = _tenantId, Hostname = "scanner.local", Message = "TCP port scan probe detected", TimestampUtc = DateTime.UtcNow },
            new() { TenantId = _tenantId, Hostname = "scanner.local", Message = "Closed port access attempt", TimestampUtc = DateTime.UtcNow }
        };

        var rule = new ThreatDetectionRule
        {
            TenantId = _tenantId,
            ThreatType = ThreatType.PortScanIndicator,
            FailureThreshold = 3,
            TimeWindowMinutes = 60,
            DefaultSeverity = ThreatSeverity.High
        };

        _mockUnitOfWork.Setup(u => u.ThreatDetectionRules.GetRuleByThreatTypeAsync(
            _tenantId, ThreatType.PortScanIndicator, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rule);

        _mockUnitOfWork.Setup(u => u.Syslogs.GetSyslogsPagedAsync(
            _tenantId, null, null, null, null, null, It.IsAny<DateTime?>(), null, 1, 1000, It.IsAny<CancellationToken>()))
            .ReturnsAsync((syslogs, syslogs.Count));

        _mockUnitOfWork.Setup(u => u.ThreatIndicators.GetActiveIndicatorAsync(
            _tenantId, ThreatType.PortScanIndicator, "scanner.local", null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ThreatIndicator?)null);

        var detector = new PortScanDetector(_mockUnitOfWork.Object, _mockTenantContext.Object);

        // Act
        var result = await detector.DetectPortScanIndicatorsAsync(timeWindowMinutes: 60);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal("scanner.local", result[0].SourceIp);
        Assert.Equal(ThreatType.PortScanIndicator, result[0].ThreatType);
    }
}