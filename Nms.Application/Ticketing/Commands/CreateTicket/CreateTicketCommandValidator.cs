using FluentValidation;

namespace Nms.Application.Ticketing.Commands.CreateTicket;

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Ticket title is required.")
            .MaximumLength(200).WithMessage("Ticket title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Ticket description is required.")
            .MaximumLength(4000).WithMessage("Ticket description cannot exceed 4000 characters.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Valid priority is required.");

        RuleFor(x => x.ProviderType)
            .IsInEnum().WithMessage("Valid ticketing provider is required.");
    }
}