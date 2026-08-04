using Nms.Application.Devices.Commands.CreateDevice;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests;

public class CreateDeviceCommandValidatorTests
{
    private readonly CreateDeviceCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new CreateDeviceCommand(
            Name: "Core-Switch-01",
            IpAddress: "192.168.1.1",
            DeviceType: DeviceType.Switch,
            SnmpPort: 161,
            MacAddress: "00:1A:2B:3C:4D:5E"
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("invalid-ip")]
    [InlineData("256.256.256.256")]
    [InlineData("10.0.0.999")]
    public void Validate_WithInvalidIpAddress_ShouldHaveValidationError(string invalidIp)
    {
        // Arrange
        var command = new CreateDeviceCommand(
            Name: "Core-Switch-01",
            IpAddress: invalidIp,
            DeviceType: DeviceType.Switch
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateDeviceCommand.IpAddress));
    }

    [Theory]
    [InlineData("00:1A:2B:3C:4D:5E")]
    [InlineData("00-1A-2B-3C-4D-5E")]
    [InlineData("001A.2B3C.4D5E")]
    public void Validate_WithValidMacAddressFormats_ShouldBeValid(string validMac)
    {
        // Arrange
        var command = new CreateDeviceCommand(
            Name: "Core-Switch-01",
            IpAddress: "10.0.0.1",
            DeviceType: DeviceType.Switch,
            MacAddress: validMac
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("INVALID-MAC")]
    [InlineData("00:1A:2B:3C:4D")]
    [InlineData("00:1A:2B:3C:4D:5E:6F")]
    public void Validate_WithInvalidMacAddress_ShouldHaveValidationError(string invalidMac)
    {
        // Arrange
        var command = new CreateDeviceCommand(
            Name: "Core-Switch-01",
            IpAddress: "10.0.0.1",
            DeviceType: DeviceType.Switch,
            MacAddress: invalidMac
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateDeviceCommand.MacAddress));
    }
}