using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.ThreatDetection.Queries.GetConfigurationDriftsPaged;
using Nms.Application.ThreatDetection.Queries.GetThreatIndicatorById;
using Nms.Application.ThreatDetection.Queries.GetThreatIndicatorsPaged;
using Nms.Application.ThreatDetection.Queries.GetThreatRulesPaged;
using Nms.Application.ThreatDetection.Queries.GetThreatSummary;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.ThreatDetection;

public class ThreatDetectionQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ITenantContext> _mockTenantContext;
    private readonly Guid _tenantId = Guid.NewGuid();

    public ThreatDetectionQueryHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTenantContext = new Mock<ITenantContext>();
        _mockTenantContext.Setup(t => t.TenantId).Returns(_tenantId);
    }

    [Fact]
    public async Task GetThreatIndicatorsPagedQueryHandler_ShouldReturnPagedResult()
    {
        // Arrange
        var indicators = new List<ThreatIndicator>
        {
            new(Guid.NewGuid()) { TenantId = _tenantId, ThreatType = ThreatType.FailedLogin, Title = "Failed Logins" }
        };

        _mockUnitOfWork.Setup(u => u.ThreatIndicators.GetPagedAsync(
            _tenantId, 1, 20, null, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((indicators, 1));

        var handler = new GetThreatIndicatorsPagedQueryHandler(_mockUnitOfWork.Object, _mockTenantContext.Object);

        // Act
        var result = await handler.Handle(new GetThreatIndicatorsPagedQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task GetThreatIndicatorByIdQueryHandler_ShouldReturnDto_WhenExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var indicator = new ThreatIndicator(id)
        {
            TenantId = _tenantId,
            ThreatType = ThreatType.ConfigurationDrift,
            Title = "Config Diverged"
        };

        _mockUnitOfWork.Setup(u => u.ThreatIndicators.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(indicator);

        var handler = new GetThreatIndicatorByIdQueryHandler(_mockUnitOfWork.Object);

        // Act
        var result = await handler.Handle(new GetThreatIndicatorByIdQuery(id), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("Config Diverged", result.Title);
    }

    [Fact]
    public async Task GetConfigurationDriftsPagedQueryHandler_ShouldReturnPagedDrifts()
    {
        // Arrange
        var drifts = new List<ConfigurationDriftRecord>
        {
            new(Guid.NewGuid()) { TenantId = _tenantId, DeviceId = Guid.NewGuid(), HasDrift = true }
        };

        _mockUnitOfWork.Setup(u => u.ConfigurationDrifts.GetPagedAsync(
            _tenantId, 1, 20, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((drifts, 1));

        var handler = new GetConfigurationDriftsPagedQueryHandler(_mockUnitOfWork.Object, _mockTenantContext.Object);

        // Act
        var result = await handler.Handle(new GetConfigurationDriftsPagedQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.True(result.Items.First().HasDrift);
    }

    [Fact]
    public async Task GetThreatRulesPagedQueryHandler_ShouldReturnRules()
    {
        // Arrange
        var rules = new List<ThreatDetectionRule>
        {
            new(Guid.NewGuid()) { TenantId = _tenantId, RuleName = "Rule 1", ThreatType = ThreatType.PortScanIndicator }
        };

        _mockUnitOfWork.Setup(u => u.ThreatDetectionRules.GetPagedAsync(
            _tenantId, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync((rules, 1));

        var handler = new GetThreatRulesPagedQueryHandler(_mockUnitOfWork.Object, _mockTenantContext.Object);

        // Act
        var result = await handler.Handle(new GetThreatRulesPagedQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Rule 1", result.Items.First().RuleName);
    }

    [Fact]
    public async Task GetThreatSummaryQueryHandler_ShouldAggregateActiveThreatCounts()
    {
        // Arrange
        var activeIndicators = new List<ThreatIndicator>
        {
            new(Guid.NewGuid()) { TenantId = _tenantId, ThreatType = ThreatType.FailedLogin, Severity = ThreatSeverity.Critical, Status = ThreatStatus.Active },
            new(Guid.NewGuid()) { TenantId = _tenantId, ThreatType = ThreatType.PortScanIndicator, Severity = ThreatSeverity.High, Status = ThreatStatus.Active }
        };

        var drifts = new List<ConfigurationDriftRecord>
        {
            new(Guid.NewGuid()) { TenantId = _tenantId, DeviceId = Guid.NewGuid(), HasDrift = true }
        };

        _mockUnitOfWork.Setup(u => u.ThreatIndicators.GetPagedAsync(
            _tenantId, 1, 1000, null, null, ThreatStatus.Active, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((activeIndicators, activeIndicators.Count));

        _mockUnitOfWork.Setup(u => u.ConfigurationDrifts.GetPagedAsync(
            _tenantId, 1, 1000, null, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((drifts, drifts.Count));

        var handler = new GetThreatSummaryQueryHandler(_mockUnitOfWork.Object, _mockTenantContext.Object);

        // Act
        var result = await handler.Handle(new GetThreatSummaryQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TotalActiveThreats);
        Assert.Equal(1, result.CriticalThreats);
        Assert.Equal(1, result.HighThreats);
        Assert.Equal(1, result.FailedLoginThreats);
        Assert.Equal(1, result.PortScanThreats);
        Assert.Equal(1, result.DevicesWithConfigDrift);
    }
}