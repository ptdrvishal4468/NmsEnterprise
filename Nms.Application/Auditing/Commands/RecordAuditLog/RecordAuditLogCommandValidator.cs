using FluentValidation;

namespace Nms.Application.Auditing.Commands.RecordAuditLog;

public class RecordAuditLogCommandValidator : AbstractValidator<RecordAuditLogCommand>
{
    public RecordAuditLogCommandValidator()
    {
        RuleFor(x => x.Action)
            .NotEmpty().WithMessage("Action is required.")
            .MaximumLength(100).WithMessage("Action cannot exceed 100 characters.");

        RuleFor(x => x.Username)
            .MaximumLength(100).WithMessage("Username cannot exceed 100 characters.");

        RuleFor(x => x.EntityName)
            .MaximumLength(100).WithMessage("Entity name cannot exceed 100 characters.");

        RuleFor(x => x.EntityId)
            .MaximumLength(100).WithMessage("Entity ID cannot exceed 100 characters.");

        RuleFor(x => x.IpAddress)
            .MaximumLength(45).WithMessage("IP address cannot exceed 45 characters.");

        RuleFor(x => x.Details)
            .MaximumLength(500).WithMessage("Details cannot exceed 500 characters.");
    }
}