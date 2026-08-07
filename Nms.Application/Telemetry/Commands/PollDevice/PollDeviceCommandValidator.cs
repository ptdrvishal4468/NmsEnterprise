using FluentValidation;

namespace Nms.Application.Telemetry.Commands.PollDevice;

public class PollDeviceCommandValidator : AbstractValidator<PollDeviceCommand>
{
    public PollDeviceCommandValidator()
    {
        RuleFor(x => x.DeviceId)
            .NotEmpty()
            .WithMessage("DeviceId is required.");
    }
}