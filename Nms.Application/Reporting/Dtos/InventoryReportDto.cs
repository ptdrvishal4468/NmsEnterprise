using Nms.Domain.Enums;

namespace Nms.Application.Reporting.Dtos;

public class InventoryReportDto
{
    public Guid DeviceId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string? Vendor { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public string? FirmwareVersion { get; set; }
    public string? Site { get; set; }
    public string? Location { get; set; }
    public int TotalInterfaces { get; set; }
    public int ActiveInterfaces { get; set; }
    public long TotalBandwidthCapacityBps { get; set; }
}