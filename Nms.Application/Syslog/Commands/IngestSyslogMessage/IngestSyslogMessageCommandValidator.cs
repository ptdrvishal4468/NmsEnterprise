using FluentValidation;

namespace Nms.Application.Syslog.Commands.IngestSyslogMessage;

public class IngestSyslogMessageCommandValidator : AbstractValidator<IngestSyslogMessageCommand>
{
    public IngestSyslogMessageCommandValidator()
    {
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.SourceIpAddress).NotEmpty().MaximumLength(45);
        RuleFor(x => x.Message).NotNull();
        RuleFor(x => x.Facility).IsInEnum();
        RuleFor(x => x.Severity).IsInEnum();
    }
}