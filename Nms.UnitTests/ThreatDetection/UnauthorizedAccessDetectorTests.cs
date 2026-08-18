using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.ThreatDetection.Services;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.ThreatDetection;

public class UnauthorizedAccessDetectorTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ITenantContext> _mockTenantContext;
    private readonly Guid _tenantId = Guid.NewGuid();

    public UnauthorizedAccessDetectorTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTenantContext = new Mock<ITenantContext>();
        _mockTenantContext.Setup(t => t.TenantId).Returns(_tenantId);
    }

    [Fact]
    public async Task DetectUnauthorizedAccessPatternsAsync_ShouldFlagUser_WhenDeniedSecurityLogsExceedThreshold()
    {
        // Arrange
        var auditLogs = new List<AuditLog>
        {
            new(_tenantId, Guid.NewGuid(), "AccessRestrictedResource", username: "unauthorized_user", category: AuditCategory.Security, status: AuditStatus.Failure),
            new(_tenantId, Guid.NewGuid(), "AccessRestrictedResource", username: "unauthorized_user", category: AuditCategory.Security, status: AuditStatus.Failure),
            new(_tenantId, Guid.NewGuid(), "AccessRestrictedResource", username: "unauthorized_user", category: AuditCategory.Security, status: AuditStatus.Failure)
        };

        _mockUnitOfWork.Setup(u => u.ThreatDetectionRules.GetRuleByThreatTypeAsync(
            _tenantId, ThreatType.UnauthorizedAccess, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ThreatDetectionRule?)null);

        _mockUnitOfWork.Setup(u => u.AuditLogs.SearchAuditLogsAsync(
            _tenantId, It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), null, null, AuditCategory.Security,
            AuditStatus.Failure, null, null, null, 1, 1000, It.IsAny<CancellationToken>()))
            .ReturnsAsync((auditLogs, auditLogs.Count));

        _mockUnitOfWork.Setup(u => u.ThreatIndicators.GetActiveIndicatorAsync(
            _tenantId, ThreatType.UnauthorizedAccess, null, null, "unauthorized_user", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ThreatIndicator?)null);

        var detector = new UnauthorizedAccessDetector(_mockUnitOfWork.Object, _mockTenantContext.Object);

        // Act
        var result = await detector.DetectUnauthorizedAccessPatternsAsync(timeWindowMinutes: 60);

        // Assert
        Assert.Single(result);
        Assert.Equal("unauthorized_user", result[0].TargetUser);
        Assert.Equal(ThreatType.UnauthorizedAccess, result[0].ThreatType);
        Assert.Equal(ThreatSeverity.Critical, result[0].Severity);
    }
}