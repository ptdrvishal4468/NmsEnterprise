using FluentValidation;
using Nms.Application.Discovery.Services;

namespace Nms.Application.Discovery.Commands.StartDiscoveryScan;

public class StartDiscoveryScanCommandValidator : AbstractValidator<StartDiscoveryScanCommand>
{
    public StartDiscoveryScanCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Discovery scan name is required.")
            .MaximumLength(100).WithMessage("Scan name must not exceed 100 characters.");

        RuleFor(x => x.IpRange)
            .NotEmpty().WithMessage("IP range or CIDR block is required.")
            .Must(BeValidIpRange).WithMessage("Invalid IP range, CIDR block, or exceeds maximum target limit (1024).");

        RuleFor(x => x.SnmpPort)
            .InclusiveBetween(1, 65535).WithMessage("SNMP port must be between 1 and 65535.");
    }

    private bool BeValidIpRange(string ipRange)
    {
        try
        {
            var ips = IpRangeCalculator.CalculateIpAddresses(ipRange);
            return ips.Count > 0;
        }
        catch
        {
            return false;
        }
    }
}