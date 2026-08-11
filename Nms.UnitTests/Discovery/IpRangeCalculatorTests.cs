using Nms.Application.Discovery.Services;
using Xunit;

namespace Nms.UnitTests.Discovery;

public class IpRangeCalculatorTests
{
    [Fact]
    public void CalculateIpAddresses_WithCidr29_ReturnsCorrectAddressCount()
    {
        // Arrange
        var cidr = "192.168.1.0/29";

        // Act
        var result = IpRangeCalculator.CalculateIpAddresses(cidr);

        // Assert
        Assert.Equal(6, result.Count);
        Assert.Contains("192.168.1.1", result);
        Assert.Contains("192.168.1.6", result);
    }

    [Fact]
    public void CalculateIpAddresses_WithExplicitRange_ReturnsCorrectAddresses()
    {
        // Arrange
        var range = "10.0.0.1-10.0.0.5";

        // Act
        var result = IpRangeCalculator.CalculateIpAddresses(range);

        // Assert
        Assert.Equal(5, result.Count);
        Assert.Equal("10.0.0.1", result.First());
        Assert.Equal("10.0.0.5", result.Last());
    }

    [Fact]
    public void CalculateIpAddresses_ExceedingMaxLimit_ThrowsInvalidOperationException()
    {
        // Arrange
        var largeCidr = "10.0.0.0/16"; // 65,534 hosts

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => IpRangeCalculator.CalculateIpAddresses(largeCidr));
    }
}