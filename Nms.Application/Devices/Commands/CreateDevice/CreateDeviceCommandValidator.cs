using FluentValidation;

namespace Nms.Application.Devices.Commands.CreateDevice;

public class CreateDeviceCommandValidator : AbstractValidator<CreateDeviceCommand>
{
    public CreateDeviceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Device name is required.")
            .MaximumLength(150).WithMessage("Device name must not exceed 150 characters.");

        RuleFor(x => x.IpAddress)
            .NotEmpty().WithMessage("IP address is required.")
            .Must(BeValidIpAddress).WithMessage("Invalid IP address format.");

        RuleFor(x => x.DeviceType)
            .IsInEnum().WithMessage("Invalid device type.");

        RuleFor(x => x.SnmpPort)
            .InclusiveBetween(1, 65535).WithMessage("SNMP port must be between 1 and 65535.");

        RuleFor(x => x.MacAddress)
            .Must(BeValidMacAddress).When(x => !string.IsNullOrWhiteSpace(x.MacAddress))
            .WithMessage("Invalid MAC address format (supported formats: 00:1A:2B:3C:4D:5E, 00-1A-2B-3C-4D-5E, or 001A.2B3C.4D5E).");

        RuleFor(x => x.Hostname)
            .MaximumLength(255).WithMessage("Hostname must not exceed 255 characters.");

        RuleFor(x => x.Vendor)
            .MaximumLength(100).WithMessage("Vendor must not exceed 100 characters.");

        RuleFor(x => x.Model)
            .MaximumLength(100).WithMessage("Model must not exceed 100 characters.");

        RuleFor(x => x.SerialNumber)
            .MaximumLength(100).WithMessage("Serial number must not exceed 100 characters.");

        RuleFor(x => x.FirmwareVersion)
            .MaximumLength(100).WithMessage("Firmware version must not exceed 100 characters.");

        RuleFor(x => x.Site)
            .MaximumLength(100).WithMessage("Site must not exceed 100 characters.");

        RuleFor(x => x.Location)
            .MaximumLength(200).WithMessage("Location must not exceed 200 characters.");
    }

    private static bool BeValidIpAddress(string ipAddress)
    {
        return System.Net.IPAddress.TryParse(ipAddress, out _);
    }

    private static bool BeValidMacAddress(string? macAddress)
    {
        if (string.IsNullOrWhiteSpace(macAddress)) return true;

        var regex = new System.Text.RegularExpressions.Regex(
            @"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$|^([0-9A-Fa-f]{4}\.[0-9A-Fa-f]{4}\.[0-9A-Fa-f]{4})$");

        return regex.IsMatch(macAddress);
    }
}