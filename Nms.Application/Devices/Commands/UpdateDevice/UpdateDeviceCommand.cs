using MediatR;
using Nms.Application.Devices.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Devices.Commands.UpdateDevice;

public record UpdateDeviceCommand(
    Guid Id,
    string Name,
    string IpAddress,
    DeviceType DeviceType,
    int SnmpPort = 161,
    string? Hostname = null,
    string? Vendor = null,
    string? Model = null,
    string? SerialNumber = null,
    string? FirmwareVersion = null,
    string? MacAddress = null,
    string? Site = null,
    string? Location = null
) : IRequest<DeviceDto>;