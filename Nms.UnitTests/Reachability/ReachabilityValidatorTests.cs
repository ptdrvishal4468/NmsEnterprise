using FluentValidation.TestHelper;
using Nms.Application.Reachability.Commands.PingDevice;
using Xunit;

namespace Nms.UnitTests.Reachability;

public class ReachabilityValidatorTests
{
    private readonly PingDeviceCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_DeviceId_Is_Empty()
    {
        var command = new PingDeviceCommand(Guid.Empty, 4, 1000);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.DeviceId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Should_Have_Error_When_PacketCount_Is_Out_Of_Range(int packetCount)
    {
        var command = new PingDeviceCommand(Guid.NewGuid(), packetCount, 1000);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PacketCount);
    }

    [Theory]
    [InlineData(50)]
    [InlineData(15000)]
    public void Should_Have_Error_When_TimeoutMs_Is_Out_Of_Range(int timeoutMs)
    {
        var command = new PingDeviceCommand(Guid.NewGuid(), 4, timeoutMs);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.TimeoutMs);
    }

    [Fact]
    public void Should_Pass_Validation_When_Inputs_Are_Valid()
    {
        var command = new PingDeviceCommand(Guid.NewGuid(), 4, 1000);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}