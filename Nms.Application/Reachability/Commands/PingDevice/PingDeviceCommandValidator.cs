using FluentValidation;

namespace Nms.Application.Reachability.Commands.PingDevice;

public sealed class PingDeviceCommandValidator : AbstractValidator<PingDeviceCommand>
{
    public PingDeviceCommandValidator()
    {
        RuleFor(x => x.DeviceId)
            .NotEmpty()
            .WithMessage("DeviceId is required.");

        RuleFor(x => x.PacketCount)
            .InclusiveBetween(1, 10)
            .WithMessage("Packet count must be between 1 and 10.");

        RuleFor(x => x.TimeoutMs)
            .InclusiveBetween(100, 10000)
            .WithMessage("Timeout must be between 100ms and 10,000ms.");
    }
}