using Nms.Application.Discovery.Services;
using Xunit;

namespace Nms.UnitTests.Discovery;

public class VendorIdentifierTests
{
    [Theory]
    [InlineData(".1.3.6.1.4.1.9.1.516", null, "Cisco Systems")]
    [InlineData(".1.3.6.1.4.1.11.2.3.7.11", null, "HP / Aruba")]
    [InlineData(".1.3.6.1.4.1.2636.1.1.1", null, "Juniper Networks")]
    [InlineData(null, "Cisco Adaptive Security Appliance", "Cisco Systems")]
    [InlineData(null, "Unknown appliance text", "Unknown")]
    public void IdentifyVendor_ReturnsExpectedVendor(string? sysObjectId, string? sysDescr, string expected)
    {
        // Act
        var result = VendorIdentifier.IdentifyVendor(sysObjectId, sysDescr);

        // Assert
        Assert.Equal(expected, result);
    }
}