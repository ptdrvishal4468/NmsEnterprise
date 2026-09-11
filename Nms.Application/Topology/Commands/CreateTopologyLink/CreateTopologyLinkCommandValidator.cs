using FluentValidation;

namespace Nms.Application.Topology.Commands.CreateTopologyLink;

public class CreateTopologyLinkCommandValidator : AbstractValidator<CreateTopologyLinkCommand>
{
    public CreateTopologyLinkCommandValidator()
    {
        RuleFor(x => x.SourceDeviceId)
            .NotEmpty().WithMessage("Source device ID is required.");

        RuleFor(x => x.TargetDeviceId)
            .NotEmpty().WithMessage("Target device ID is required.")
            .NotEqual(x => x.SourceDeviceId).WithMessage("Source and target devices cannot be the same.");

        RuleFor(x => x.SpeedBps)
            .GreaterThanOrEqualTo(0).WithMessage("Speed must be non-negative.");
    }
}