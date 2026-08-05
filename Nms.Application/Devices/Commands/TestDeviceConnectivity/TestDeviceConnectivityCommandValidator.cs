using FluentValidation;

namespace Nms.Application.Devices.Commands.TestDeviceConnectivity;

public sealed class TestDeviceConnectivityCommandValidator : AbstractValidator<TestDeviceConnectivityCommand>
{
    public TestDeviceConnectivityCommandValidator()
    {
        RuleFor(x => x.DeviceId)
            .NotEmpty()
            .WithMessage("Device ID is required.");

        RuleFor(x => x.Protocol)
            .IsInEnum()
            .WithMessage("A valid network protocol must be specified.");

        RuleFor(x => x.TimeoutMs)
            .InclusiveBetween(100, 10000)
            .WithMessage("Timeout must be between 100ms and 10,000ms.");
    }
}