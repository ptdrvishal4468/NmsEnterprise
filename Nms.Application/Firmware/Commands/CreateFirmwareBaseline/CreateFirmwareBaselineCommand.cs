using MediatR;
using Nms.Application.Firmware.Dtos;

namespace Nms.Application.Firmware.Commands.CreateFirmwareBaseline;

public record CreateFirmwareBaselineCommand(
    string Vendor,
    string Model,
    string TargetVersion,
    string? Notes = null,
    bool IsActive = true) : IRequest<FirmwareBaselineDto>;