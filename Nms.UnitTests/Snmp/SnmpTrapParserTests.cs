using Nms.Infrastructure.Snmp;
using Xunit;

namespace Nms.UnitTests.Snmp;

public class SnmpTrapParserTests
{
    private readonly SnmpTrapParser _parser = new();

    [Fact]
    public void Parse_MalformedPayload_ReturnsMalformedTrapResult()
    {
        byte[] malformedPayload = new byte[] { 0x30, 0x03, 0x00, 0x00 };

        var result = _parser.Parse(malformedPayload, "192.168.1.100");

        Assert.NotNull(result);
        Assert.True(result.IsMalformed);
        Assert.Equal("UNKNOWN", result.EnterpriseOid);
        Assert.Equal("192.168.1.100", result.SourceIpAddress);
    }
}