using FluentValidation;

namespace Nms.Application.SnmpTraps.Commands.IngestSnmpTrap;

public sealed class IngestSnmpTrapCommandValidator : AbstractValidator<IngestSnmpTrapCommand>
{
    public IngestSnmpTrapCommandValidator()
    {
        RuleFor(x => x.SourceIpAddress).NotEmpty().WithMessage("Source IP address is required.");
        RuleFor(x => x.EnterpriseOid).NotEmpty().When(x => !x.IsMalformed).WithMessage("Enterprise OID is required for valid traps.");
    }
}