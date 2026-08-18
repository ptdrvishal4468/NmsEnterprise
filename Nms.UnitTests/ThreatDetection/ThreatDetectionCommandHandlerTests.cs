using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.ThreatDetection.Commands.AnalyzeConfigurationDrift;
using Nms.Application.ThreatDetection.Commands.AnalyzeFailedLogins;
using Nms.Application.ThreatDetection.Commands.AnalyzePortScans;
using Nms.Application.ThreatDetection.Commands.AnalyzeUnauthorizedAccess;
using Nms.Application.ThreatDetection.Commands.CreateThreatRule;
using Nms.Application.ThreatDetection.Commands.UpdateThreatIndicatorStatus;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.ThreatDetection;

public class ThreatDetectionCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ITenantContext> _mockTenantContext;
    private readonly Guid _tenantId = Guid.NewGuid();

    public ThreatDetectionCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTenantContext = new Mock<ITenantContext>();
        _mockTenantContext.Setup(t => t.TenantId).Returns(_tenantId);
    }

    [Fact]
    public async Task CreateThreatRuleCommandHandler_ShouldPersistAndReturnDto()
    {
        // Arrange
        var command = new CreateThreatRuleCommand(
            RuleName: "High Volume Failed Logins",
            ThreatType: ThreatType.FailedLogin,
            DefaultSeverity: ThreatSeverity.High,
            FailureThreshold: 5,
            TimeWindowMinutes: 10,
            IsEnabled: true,
            Description: "Detects brute force login attempts");

        _mockUnitOfWork.Setup(u => u.ThreatDetectionRules.AddAsync(It.IsAny<ThreatDetectionRule>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreateThreatRuleCommandHandler(_mockUnitOfWork.Object, _mockTenantContext.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("High Volume Failed Logins", result.RuleName);
        Assert.Equal(ThreatType.FailedLogin, result.ThreatType);
        Assert.Equal(5, result.FailureThreshold);
        _mockUnitOfWork.Verify(u => u.ThreatDetectionRules.AddAsync(It.IsAny<ThreatDetectionRule>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateThreatIndicatorStatusCommandHandler_ShouldUpdateStatusAndResolutionNotes()
    {
        // Arrange
        var indicatorId = Guid.NewGuid();
        var indicator = new ThreatIndicator(indicatorId)
        {
            TenantId = _tenantId,
            ThreatType = ThreatType.FailedLogin,
            Severity = ThreatSeverity.High,
            Status = ThreatStatus.Active,
            Title = "Brute force attack"
        };

        _mockUnitOfWork.Setup(u => u.ThreatIndicators.GetByIdAsync(indicatorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(indicator);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new UpdateThreatIndicatorStatusCommand(indicatorId, ThreatStatus.Resolved, "Blocked source IP address");
        var handler = new UpdateThreatIndicatorStatusCommandHandler(_mockUnitOfWork.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ThreatStatus.Resolved, result.Status);
        Assert.Equal("Blocked source IP address", result.ResolutionNotes);
        _mockUnitOfWork.Verify(u => u.ThreatIndicators.Update(indicator), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AnalyzeFailedLoginsCommandHandler_ShouldInvokeDetector()
    {
        // Arrange
        var mockDetector = new Mock<IFailedLoginDetector>();
        var indicators = new List<ThreatIndicator>
        {
            new(Guid.NewGuid()) { TenantId = _tenantId, ThreatType = ThreatType.FailedLogin, SourceIp = "10.0.0.1" }
        };
        mockDetector.Setup(d => d.DetectFailedLoginsAsync(null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(indicators);

        var handler = new AnalyzeFailedLoginsCommandHandler(mockDetector.Object);

        // Act
        var result = await handler.Handle(new AnalyzeFailedLoginsCommand(), CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("10.0.0.1", result[0].SourceIp);
    }

    [Fact]
    public async Task AnalyzeConfigurationDriftCommandHandler_ShouldInvokeDetector()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var mockDriftDetector = new Mock<IConfigurationDriftDetector>();
        var record = new ConfigurationDriftRecord(Guid.NewGuid())
        {
            TenantId = _tenantId,
            DeviceId = deviceId,
            HasDrift = true,
            AddedLinesCount = 3,
            RemovedLinesCount = 1
        };

        mockDriftDetector.Setup(d => d.AnalyzeDeviceDriftAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(record);

        var handler = new AnalyzeConfigurationDriftCommandHandler(mockDriftDetector.Object);

        // Act
        var result = await handler.Handle(new AnalyzeConfigurationDriftCommand(deviceId), CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.True(result[0].HasDrift);
        Assert.Equal(3, result[0].AddedLinesCount);
    }

    [Fact]
    public async Task AnalyzePortScansCommandHandler_ShouldInvokeDetector()
    {
        // Arrange
        var mockDetector = new Mock<IPortScanDetector>();
        var indicators = new List<ThreatIndicator>
        {
            new(Guid.NewGuid()) { TenantId = _tenantId, ThreatType = ThreatType.PortScanIndicator, SourceIp = "192.168.1.50" }
        };

        mockDetector.Setup(d => d.DetectPortScanIndicatorsAsync(60, It.IsAny<CancellationToken>()))
            .ReturnsAsync(indicators);

        var handler = new AnalyzePortScansCommandHandler(mockDetector.Object);

        // Act
        var result = await handler.Handle(new AnalyzePortScansCommand(60), CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("192.168.1.50", result[0].SourceIp);
    }

    [Fact]
    public async Task AnalyzeUnauthorizedAccessCommandHandler_ShouldInvokeDetector()
    {
        // Arrange
        var mockDetector = new Mock<IUnauthorizedAccessDetector>();
        var indicators = new List<ThreatIndicator>
        {
            new(Guid.NewGuid()) { TenantId = _tenantId, ThreatType = ThreatType.UnauthorizedAccess, TargetUser = "bad_actor" }
        };

        mockDetector.Setup(d => d.DetectUnauthorizedAccessPatternsAsync(60, It.IsAny<CancellationToken>()))
            .ReturnsAsync(indicators);

        var handler = new AnalyzeUnauthorizedAccessCommandHandler(mockDetector.Object);

        // Act
        var result = await handler.Handle(new AnalyzeUnauthorizedAccessCommand(60), CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("bad_actor", result[0].TargetUser);
    }
}