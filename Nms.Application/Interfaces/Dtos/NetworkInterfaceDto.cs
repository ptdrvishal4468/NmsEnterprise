using Nms.Domain.Enums;

namespace Nms.Application.Interfaces.Dtos;

public class NetworkInterfaceDto
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public int IfIndex { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? InterfaceType { get; set; }
    public string? MacAddress { get; set; }
    public long SpeedBps { get; set; }
    public InterfaceAdminStatus AdminStatus { get; set; }
    public InterfaceOperStatus OperStatus { get; set; }
    public InterfaceDuplex Duplex { get; set; }
    public long InOctets { get; set; }
    public long OutOctets { get; set; }
    public long InErrors { get; set; }
    public long OutErrors { get; set; }
    public long InDiscards { get; set; }
    public long OutDiscards { get; set; }
    public double UtilizationPercent { get; set; }
    public DateTime? LastPolledUtc { get; set; }
}