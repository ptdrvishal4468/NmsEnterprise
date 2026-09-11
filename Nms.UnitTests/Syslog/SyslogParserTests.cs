using Nms.Domain.Enums;
using Nms.Infrastructure.Syslog;
using Xunit;

namespace Nms.UnitTests.Syslog;

public class SyslogParserTests
{
    private readonly SyslogParser _parser = new();

    [Fact]
    public void Parse_Rfc3164_ValidMessage_ReturnsCorrectFields()
    {
        // Arrange: PRI 34 = Facility 4 (Auth), Severity 2 (Critical)
        var raw = "<34>Oct 11 22:14:15 myhost myapp[1234]: User authentication failed";

        // Act
        var result = _parser.Parse(raw, "192.168.1.100");

        // Assert
        Assert.False(result.IsMalformed);
        Assert.Equal(SyslogFacility.Auth, result.Facility);
        Assert.Equal(SyslogSeverity.Critical, result.Severity);
        Assert.Equal("myhost", result.Hostname);
        Assert.Equal("myapp", result.AppTag);
        Assert.Equal("1234", result.ProcessId);
        Assert.Equal("User authentication failed", result.Message);
    }

    [Fact]
    public void Parse_Rfc5424_ValidMessage_ReturnsCorrectFields()
    {
        // Arrange: PRI 165 = Facility 20 (Local4), Severity 5 (Notice)
        var raw = "<165>1 2026-08-12T10:00:00.000Z myrouter appName 5678 ID47 [exampleSDID@32473 iut=\"3\"] Network interface link status changed";

        // Act
        var result = _parser.Parse(raw, "10.0.0.1");

        // Assert
        Assert.False(result.IsMalformed);
        Assert.Equal(SyslogFacility.Local4, result.Facility);
        Assert.Equal(SyslogSeverity.Notice, result.Severity);
        Assert.Equal("myrouter", result.Hostname);
        Assert.Equal("appName", result.AppTag);
        Assert.Equal("5678", result.ProcessId);
        Assert.Equal("ID47", result.MessageId);
        Assert.Equal("Network interface link status changed", result.Message);
    }

    [Fact]
    public void Parse_MalformedMessage_ReturnsFallbackWithIsMalformedTrue()
    {
        // Arrange
        var raw = "Unstructured raw text log without RFC header format";

        // Act
        var result = _parser.Parse(raw, "192.168.1.50");

        // Assert
        Assert.True(result.IsMalformed);
        Assert.Equal(SyslogFacility.Local7, result.Facility);
        Assert.Equal(SyslogSeverity.Notice, result.Severity);
        Assert.Equal("Unstructured raw text log without RFC header format", result.Message);
    }

    [Fact]
    public void Parse_EmptyOrNullMessage_ReturnsMalformedFallback()
    {
        // Act
        var result = _parser.Parse(string.Empty, "127.0.0.1");

        // Assert
        Assert.True(result.IsMalformed);
        Assert.Equal(SyslogFacility.Local7, result.Facility);
        Assert.Equal(SyslogSeverity.Notice, result.Severity);
    }
}