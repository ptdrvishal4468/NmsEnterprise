using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.ThreatDetection.Services;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.ThreatDetection;

public class FailedLoginDetectorTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ITenantContext> _mockTenantContext;
    private readonly Guid _tenantId = Guid.NewGuid();

    public FailedLoginDetectorTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTenantContext = new Mock<ITenantContext>();
        _mockTenantContext.Setup(t => t.TenantId).Returns(_tenantId);
    }

    [Fact]
    public async Task DetectFailedLoginsAsync_ShouldCreateThreatIndicator_WhenThresholdExceeded()
    {
        // Arrange
        var auditLogs = new List<AuditLog>
        {
            new(_tenantId, Guid.NewGuid(), "Login", ipAddress: "192.168.1.100", username: "admin", category: AuditCategory.Authentication, status: AuditStatus.Failure),
            new(_tenantId, Guid.NewGuid(), "Login", ipAddress: "192.168.1.100", username: "admin", category: AuditCategory.Authentication, status: AuditStatus.Failure),
            new(_tenantId, Guid.NewGuid(), "Login", ipAddress: "192.168.1.100", username: "admin", category: AuditCategory.Authentication, status: AuditStatus.Failure)
        };

        _mockUnitOfWork.Setup(u => u.ThreatDetectionRules.GetRuleByThreatTypeAsync(
            _tenantId, ThreatType.FailedLogin, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ThreatDetectionRule?)null);

        _mockUnitOfWork.Setup(u => u.AuditLogs.SearchAuditLogsAsync(
            _tenantId, It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), null, null, AuditCategory.Authentication,
            AuditStatus.Failure, null, null, null, 1, 1000, It.IsAny<CancellationToken>()))
            .ReturnsAsync((auditLogs, auditLogs.Count));

        _mockUnitOfWork.Setup(u => u.ThreatIndicators.GetActiveIndicatorAsync(
            _tenantId, ThreatType.FailedLogin, "192.168.1.100", null, "admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ThreatIndicator?)null);

        var detector = new FailedLoginDetector(_mockUnitOfWork.Object, _mockTenantContext.Object);

        // Act
        var result = await detector.DetectFailedLoginsAsync(thresholdOverride: 3, timeWindowMinutesOverride: 15);

        // Assert
        Assert.Single(result);
        Assert.Equal("192.168.1.100", result[0].SourceIp);
        Assert.Equal(3, result[0].AttemptCount);
        Assert.Equal(ThreatSeverity.High, result[0].Severity);
        _mockUnitOfWork.Verify(u => u.ThreatIndicators.AddAsync(It.IsAny<ThreatIndicator>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}