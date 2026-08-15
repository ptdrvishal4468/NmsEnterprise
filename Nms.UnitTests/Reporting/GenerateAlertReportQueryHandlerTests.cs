using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Reporting.Queries.GenerateAlertReport;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Reporting;

public class GenerateAlertReportQueryHandlerTests
{
    private readonly Mock<IAlertRepository> _alertRepoMock;
    private readonly Mock<IDeviceRepository> _deviceRepoMock;
    private readonly Mock<ICsvReportFormatter> _csvFormatterMock;
    private readonly GenerateAlertReportQueryHandler _handler;

    public GenerateAlertReportQueryHandlerTests()
    {
        _alertRepoMock = new Mock<IAlertRepository>();
        _deviceRepoMock = new Mock<IDeviceRepository>();
        _csvFormatterMock = new Mock<ICsvReportFormatter>();
        _handler = new GenerateAlertReportQueryHandler(_alertRepoMock.Object, _deviceRepoMock.Object, _csvFormatterMock.Object);
    }

    [Fact]
    public async Task Handle_FiltersBySeverityAndState_ReturnsMatchingAlerts()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var devId = Guid.NewGuid();

        var devices = new List<Device> { new(devId, tenantId, "Router-1", "10.0.0.1", DeviceType.Router) };

        var alerts = new List<Alert>
        {
            new(tenantId, Guid.NewGuid(), devId, MetricType.CpuUsage, AlertSeverity.Critical, 95m, 90m, "CPU threshold breached"),
            new(tenantId, Guid.NewGuid(), devId, MetricType.MemoryUsage, AlertSeverity.Warning, 85m, 80m, "Memory threshold breached")
        };

        _deviceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(devices);
        _alertRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(alerts);

        var query = new GenerateAlertReportQuery(Severity: AlertSeverity.Critical, Format: ReportFormat.Json);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalRecords);
    }
}