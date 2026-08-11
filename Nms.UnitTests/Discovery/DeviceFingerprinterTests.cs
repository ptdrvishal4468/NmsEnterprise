using Nms.Application.Discovery.Services;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests.Discovery;

public class DeviceFingerprinterTests
{
    [Theory]
    [InlineData("Cisco IOS Software, C3560 Software (C3560-IPBASEK9-M)", DeviceType.Switch)]
    [InlineData("FortiGate-60E v6.2.3,build1066", DeviceType.Firewall)]
    [InlineData("Linux ubuntu-server 5.4.0-42-generic #46-Ubuntu", DeviceType.LinuxServer)]
    [InlineData("Hardware: Intel64 Family 6 Model 158 ~ Microsoft Windows Server 2019", DeviceType.WindowsServer)]
    [InlineData("Unknown device response", DeviceType.Unknown)]
    public void FingerprintDevice_ReturnsExpectedDeviceType(string sysDescr, DeviceType expected)
    {
        // Act
        var result = DeviceFingerprinter.FingerprintDevice(null, sysDescr);

        // Assert
        Assert.Equal(expected, result);
    }
}