using Moq;
using Nms.Application.Alerts.Services;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Alerts;

public class AlertEvaluationEngineTests
{
    private readonly Mock<IAlertRuleRepository> _ruleRepoMock = new();
    private readonly Mock<IAlertRepository> _alertRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly AlertEvaluationEngine _sut;

    public AlertEvaluationEngineTests()
    {
        _sut = new AlertEvaluationEngine(_ruleRepoMock.Object, _alertRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task EvaluateMetricsAsync_WhenThresholdBreachedAndNoActiveAlert_CreatesNewAlert()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var rule = new AlertRule(tenantId, "High CPU", "CPU > 80", MetricType.CpuUsage, ComparisonOperator.GreaterThan, 80m, AlertSeverity.Critical, deviceId);

        _ruleRepoMock.Setup(r => r.GetActiveRulesForDeviceAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { rule });

        _alertRepoMock.Setup(a => a.GetActiveAlertByRuleAndDeviceAsync(rule.Id, deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Alert?)null);

        var metric = new DeviceMetricRaw(tenantId, deviceId, 85m, 50m, 50m, 10m, 40m, 1, 1, 10);

        // Act
        await _sut.EvaluateMetricsAsync(tenantId, deviceId, new[] { metric }, CancellationToken.None);

        // Assert
        _alertRepoMock.Verify(a => a.AddAsync(It.Is<Alert>(x => x.MetricValue == 85m && x.Severity == AlertSeverity.Critical), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EvaluateMetricsAsync_WhenThresholdBreachedAndActiveAlertExists_RecordsRepeatedBreachWithoutDuplicate()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var rule = new AlertRule(tenantId, "High CPU", "CPU > 80", MetricType.CpuUsage, ComparisonOperator.GreaterThan, 80m, AlertSeverity.Critical, deviceId);
        var existingAlert = new Alert(tenantId, rule.Id, deviceId, MetricType.CpuUsage, AlertSeverity.Critical, 82m, 80m, "High CPU");

        _ruleRepoMock.Setup(r => r.GetActiveRulesForDeviceAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { rule });

        _alertRepoMock.Setup(a => a.GetActiveAlertByRuleAndDeviceAsync(rule.Id, deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAlert);

        var metric = new DeviceMetricRaw(tenantId, deviceId, 88m, 50m, 50m, 10m, 40m, 1, 1, 10);

        // Act
        await _sut.EvaluateMetricsAsync(tenantId, deviceId, new[] { metric }, CancellationToken.None);

        // Assert
        _alertRepoMock.Verify(a => a.Update(It.Is<Alert>(x => x.MetricValue == 88m)), Times.Once);
        _alertRepoMock.Verify(a => a.AddAsync(It.IsAny<Alert>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EvaluateMetricsAsync_WhenThresholdNotBreachedAndActiveAlertExists_ResolvesAlert()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var rule = new AlertRule(tenantId, "High CPU", "CPU > 80", MetricType.CpuUsage, ComparisonOperator.GreaterThan, 80m, AlertSeverity.Critical, deviceId);
        var existingAlert = new Alert(tenantId, rule.Id, deviceId, MetricType.CpuUsage, AlertSeverity.Critical, 85m, 80m, "High CPU");

        _ruleRepoMock.Setup(r => r.GetActiveRulesForDeviceAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { rule });

        _alertRepoMock.Setup(a => a.GetActiveAlertByRuleAndDeviceAsync(rule.Id, deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAlert);

        var metric = new DeviceMetricRaw(tenantId, deviceId, 50m, 50m, 50m, 10m, 40m, 1, 1, 10);

        // Act
        await _sut.EvaluateMetricsAsync(tenantId, deviceId, new[] { metric }, CancellationToken.None);

        // Assert
        Assert.Equal(AlertState.Resolved, existingAlert.State);
        _alertRepoMock.Verify(a => a.Update(existingAlert), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}