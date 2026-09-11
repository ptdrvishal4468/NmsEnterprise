using System.Text;
using Nms.Application.Reporting.Services;
using Xunit;

namespace Nms.UnitTests.Reporting;

public class CsvReportFormatterTests
{
    private class SampleReportItem
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public string? Notes { get; set; }
        public DateTime Timestamp { get; set; }
    }

    [Fact]
    public void FormatToCsv_WithValidRecords_GeneratesExpectedCsvStructure()
    {
        // Arrange
        var formatter = new CsvReportFormatter();
        var records = new List<SampleReportItem>
        {
            new() { Name = "Router-01", Count = 10, Notes = "Core, Edge", Timestamp = new DateTime(2026, 8, 15, 12, 0, 0, DateTimeKind.Utc) },
            new() { Name = "Switch-02", Count = 5, Notes = "Access \"Tier\"", Timestamp = new DateTime(2026, 8, 15, 13, 0, 0, DateTimeKind.Utc) }
        };

        // Act
        var csvBytes = formatter.FormatToCsv(records);
        var csvString = Encoding.UTF8.GetString(csvBytes);

        // Assert
        Assert.NotNull(csvBytes);
        Assert.NotEmpty(csvBytes);
        Assert.Contains("Name,Count,Notes,Timestamp", csvString);
        Assert.Contains("Router-01,10,\"Core, Edge\"", csvString);
        Assert.Contains("Switch-02,5,\"Access \"\"Tier\"\"\"", csvString);
    }

    [Fact]
    public void FormatToCsv_WithEmptyList_GeneratesHeadersOnly()
    {
        // Arrange
        var formatter = new CsvReportFormatter();
        var records = new List<SampleReportItem>();

        // Act
        var csvBytes = formatter.FormatToCsv(records);
        var csvString = Encoding.UTF8.GetString(csvBytes);

        // Assert
        Assert.NotNull(csvBytes);
        Assert.Contains("Name,Count,Notes,Timestamp", csvString);
        var lines = csvString.Trim().Split(Environment.NewLine);
        Assert.Single(lines);
    }
}