using System.Text;
using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Reporting.Queries.GenerateDeviceReport;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Reporting;

public class GenerateDeviceReportQueryHandlerTests
{
    private readonly Mock<IDeviceRepository> _deviceRepositoryMock;
    private readonly Mock<ICsvReportFormatter> _csvFormatterMock;
    private readonly GenerateDeviceReportQueryHandler _handler;

    public GenerateDeviceReportQueryHandlerTests()
    {
        _deviceRepositoryMock = new Mock<IDeviceRepository>();
        _csvFormatterMock = new Mock<ICsvReportFormatter>();
        _handler = new GenerateDeviceReportQueryHandler(_deviceRepositoryMock.Object, _csvFormatterMock.Object);
    }

    [Fact]
    public async Task Handle_WithJsonFormat_ReturnsValidExportResult()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var devices = new List<Device>
        {
            new(Guid.NewGuid(), tenantId, "Router-Core", "192.168.1.1", DeviceType.Router, vendor: "Cisco", site: "HQ"),
            new(Guid.NewGuid(), tenantId, "Switch-Floor1", "192.168.1.2", DeviceType.Switch, vendor: "Juniper", site: "Branch")
        };

        _deviceRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(devices);

        var query = new GenerateDeviceReportQuery(DeviceType: DeviceType.Router, Format: ReportFormat.Json);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("application/json", result.ContentType);
        Assert.Equal(1, result.TotalRecords);
        Assert.True(result.Data.Length > 0);
    }

    [Fact]
    public async Task Handle_WithCsvFormat_CallsCsvFormatter()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var devices = new List<Device>
        {
            new(Guid.NewGuid(), tenantId, "Router-Core", "192.168.1.1", DeviceType.Router, vendor: "Cisco")
        };

        _deviceRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(devices);

        _csvFormatterMock.Setup(f => f.FormatToCsv(It.IsAny<IEnumerable<object>>()))
            .Returns(Encoding.UTF8.GetBytes("Header\nRow"));

        var query = new GenerateDeviceReportQuery(Format: ReportFormat.Csv);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("text/csv", result.ContentType);
        Assert.Equal(1, result.TotalRecords);
    }
}