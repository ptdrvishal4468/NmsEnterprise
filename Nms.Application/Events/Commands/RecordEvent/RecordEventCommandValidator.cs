using FluentValidation;

namespace Nms.Application.Events.Commands.RecordEvent;

public class RecordEventCommandValidator : AbstractValidator<RecordEventCommand>
{
    public RecordEventCommandValidator()
    {
        RuleFor(x => x.Source)
            .NotEmpty().WithMessage("Source is required.")
            .MaximumLength(200).WithMessage("Source cannot exceed 200 characters.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.")
            .MaximumLength(2000).WithMessage("Message cannot exceed 2000 characters.");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid event category.");

        RuleFor(x => x.Severity)
            .IsInEnum().WithMessage("Invalid event severity.");

        When(x => !string.IsNullOrEmpty(x.CorrelationId), () =>
        {
            RuleFor(x => x.CorrelationId)
                .MaximumLength(100).WithMessage("CorrelationId cannot exceed 100 characters.");
        });
    }
}