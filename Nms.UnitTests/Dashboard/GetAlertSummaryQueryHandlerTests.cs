using System.Linq.Expressions;
using Moq;
using Nms.Application.Dashboard.Queries.GetAlertSummary;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Dashboard;

public class GetAlertSummaryQueryHandlerTests
{
    private readonly Mock<IAlertRepository> _alertRepoMock = new();
    private readonly Mock<IAlertRuleRepository> _ruleRepoMock = new();
    private readonly Guid _tenantId = Guid.NewGuid();

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_ShouldComputeAlertSummary_WithActiveAcknowledgedAndSuppressed()
    {
        var deviceId = Guid.NewGuid();
        var rule1 = new AlertRule(_tenantId, "High CPU Alert", "Trigger on high CPU", MetricType.CpuUsage, ComparisonOperator.GreaterThan, 90m, AlertSeverity.Critical);
        var rule2 = new AlertRule(_tenantId, "High RAM Alert", "Trigger on high RAM", MetricType.MemoryUsage, ComparisonOperator.GreaterThan, 85m, AlertSeverity.Warning);

        var a1 = new Alert(_tenantId, rule1.Id, deviceId, MetricType.CpuUsage, AlertSeverity.Critical, 95m, 90m, "CPU 95%");
        var a2 = new Alert(_tenantId, rule1.Id, deviceId, MetricType.CpuUsage, AlertSeverity.Critical, 92m, 90m, "CPU 92%");
        var a3 = new Alert(_tenantId, rule2.Id, deviceId, MetricType.MemoryUsage, AlertSeverity.Warning, 88m, 85m, "RAM 88%");

        var a4 = new Alert(_tenantId, rule2.Id, deviceId, MetricType.MemoryUsage, AlertSeverity.Warning, 89m, 85m, "RAM 89%");
        a4.Acknowledge("admin@nms.local");

        var a5 = new Alert(_tenantId, rule2.Id, deviceId, MetricType.MemoryUsage, AlertSeverity.Warning, 90m, 85m, "RAM 90%");
        a5.Suppress("operator@nms.local");

        _alertRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Alert, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Alert> { a1, a2, a3, a4, a5 });

        _ruleRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AlertRule> { rule1, rule2 });

        var handler = new GetAlertSummaryQueryHandler(_alertRepoMock.Object, _ruleRepoMock.Object);
        var result = await handler.Handle(new GetAlertSummaryQuery(TopRulesLimit: 5), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.TotalActiveAlerts);
        Assert.Equal(1, result.TotalAcknowledgedAlerts);
        Assert.Equal(1, result.TotalSuppressedAlerts);
        Assert.Equal(2, result.SeverityDistribution[AlertSeverity.Critical.ToString()]);
        Assert.Equal(1, result.SeverityDistribution[AlertSeverity.Warning.ToString()]);
        Assert.Equal(2, result.MetricTypeDistribution[MetricType.CpuUsage.ToString()]);
        Assert.Equal(1, result.MetricTypeDistribution[MetricType.MemoryUsage.ToString()]);
        Assert.Equal(2, result.TopFiringRules.Count);
        Assert.Equal("High CPU Alert", result.TopFiringRules[0].RuleName);
        Assert.Equal(2, result.TopFiringRules[0].ActiveAlertCount);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_ShouldReturnEmptySummary_WhenNoAlertsExist()
    {
        _alertRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Alert, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Alert>());

        _ruleRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AlertRule>());

        var handler = new GetAlertSummaryQueryHandler(_alertRepoMock.Object, _ruleRepoMock.Object);
        var result = await handler.Handle(new GetAlertSummaryQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(0, result.TotalActiveAlerts);
        Assert.Equal(0, result.TotalAcknowledgedAlerts);
        Assert.Equal(0, result.TotalSuppressedAlerts);
        Assert.Empty(result.TopFiringRules);
    }
}