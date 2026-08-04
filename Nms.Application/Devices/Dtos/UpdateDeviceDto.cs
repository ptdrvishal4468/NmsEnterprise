using Nms.Domain.Enums;

namespace Nms.Application.Devices.Dtos;

public class UpdateDeviceDto
{
    public string Name { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string? Hostname { get; set; }
    public string? Vendor { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public string? FirmwareVersion { get; set; }
    public string? MacAddress { get; set; }
    public string? Site { get; set; }
    public string? Location { get; set; }
    public DeviceType DeviceType { get; set; }
    public int SnmpPort { get; set; } = 161;
}