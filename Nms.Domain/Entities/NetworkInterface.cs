using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class NetworkInterface : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid DeviceId { get; private set; }
    public Device? Device { get; private set; }

    public int IfIndex { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? InterfaceType { get; private set; }
    public string? MacAddress { get; private set; }

    public long SpeedBps { get; private set; }
    public InterfaceAdminStatus AdminStatus { get; private set; } = InterfaceAdminStatus.Down;
    public InterfaceOperStatus OperStatus { get; private set; } = InterfaceOperStatus.Down;
    public InterfaceDuplex Duplex { get; private set; } = InterfaceDuplex.Unknown;

    public long InOctets { get; private set; }
    public long OutOctets { get; private set; }
    public long InErrors { get; private set; }
    public long OutErrors { get; private set; }
    public long InDiscards { get; private set; }
    public long OutDiscards { get; private set; }

    public double UtilizationPercent { get; private set; }
    public DateTime? LastPolledUtc { get; private set; }

    // EF Core private constructor
    private NetworkInterface() { }

    public NetworkInterface(
        Guid id,
        Guid tenantId,
        Guid deviceId,
        int ifIndex,
        string name,
        string? description = null,
        string? interfaceType = null,
        string? macAddress = null,
        long speedBps = 0,
        InterfaceAdminStatus adminStatus = InterfaceAdminStatus.Down,
        InterfaceOperStatus operStatus = InterfaceOperStatus.Down,
        InterfaceDuplex duplex = InterfaceDuplex.Unknown) : base(id)
    {
        if (ifIndex <= 0)
            throw new ArgumentOutOfRangeException(nameof(ifIndex), "Interface index must be greater than zero.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Interface name is required.", nameof(name));

        TenantId = tenantId;
        DeviceId = deviceId;
        IfIndex = ifIndex;
        Name = name;
        Description = description;
        InterfaceType = interfaceType;
        MacAddress = macAddress;
        SpeedBps = speedBps;
        AdminStatus = adminStatus;
        OperStatus = operStatus;
        Duplex = duplex;
    }

    public void UpdateDetails(
        string name,
        string? description,
        string? interfaceType,
        string? macAddress,
        long speedBps,
        InterfaceAdminStatus adminStatus,
        InterfaceOperStatus operStatus,
        InterfaceDuplex duplex)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Interface name is required.", nameof(name));

        Name = name;
        Description = description;
        InterfaceType = interfaceType;
        MacAddress = macAddress;
        SpeedBps = speedBps;
        AdminStatus = adminStatus;
        OperStatus = operStatus;
        Duplex = duplex;
    }

    public void UpdateMetrics(
        long inOctets,
        long outOctets,
        long inErrors,
        long outErrors,
        long inDiscards,
        long outDiscards,
        double utilizationPercent,
        DateTime polledUtc)
    {
        InOctets = inOctets;
        OutOctets = outOctets;
        InErrors = inErrors;
        OutErrors = outErrors;
        InDiscards = inDiscards;
        OutDiscards = outDiscards;
        UtilizationPercent = utilizationPercent;
        LastPolledUtc = polledUtc;
    }
}