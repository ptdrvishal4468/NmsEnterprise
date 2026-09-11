using FluentValidation;

namespace Nms.Application.Reporting.Commands.CreateScheduledReport;

public class CreateScheduledReportCommandValidator : AbstractValidator<CreateScheduledReportCommand>
{
    public CreateScheduledReportCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Report name is required.")
            .MaximumLength(150).WithMessage("Report name cannot exceed 150 characters.");

        RuleFor(x => x.RecipientEmail)
            .NotEmpty().WithMessage("Recipient email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(256).WithMessage("Recipient email cannot exceed 256 characters.");

        RuleFor(x => x.ReportType)
            .IsInEnum().WithMessage("A valid report type must be specified.");

        RuleFor(x => x.ScheduleFrequency)
            .IsInEnum().WithMessage("A valid schedule frequency must be specified.");

        RuleFor(x => x.OutputFormat)
            .IsInEnum().WithMessage("A valid output format must be specified.");
    }
}