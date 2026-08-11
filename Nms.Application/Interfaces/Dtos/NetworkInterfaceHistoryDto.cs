using Nms.Domain.Enums;

namespace Nms.Application.Interfaces.Dtos;

public class NetworkInterfaceHistoryDto
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public Guid NetworkInterfaceId { get; set; }
    public int IfIndex { get; set; }
    public InterfaceAdminStatus AdminStatus { get; set; }
    public InterfaceOperStatus OperStatus { get; set; }
    public long SpeedBps { get; set; }
    public long InOctets { get; set; }
    public long OutOctets { get; set; }
    public double UtilizationPercent { get; set; }
    public DateTime TimestampUtc { get; set; }
}