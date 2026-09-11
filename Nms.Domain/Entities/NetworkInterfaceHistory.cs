using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class NetworkInterfaceHistory : BaseEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid DeviceId { get; private set; }
    public Guid NetworkInterfaceId { get; private set; }
    public NetworkInterface? NetworkInterface { get; private set; }

    public int IfIndex { get; private set; }
    public InterfaceAdminStatus AdminStatus { get; private set; }
    public InterfaceOperStatus OperStatus { get; private set; }
    public long SpeedBps { get; private set; }

    public long InOctets { get; private set; }
    public long OutOctets { get; private set; }
    public long InErrors { get; private set; }
    public long OutErrors { get; private set; }
    public long InDiscards { get; private set; }
    public long OutDiscards { get; private set; }

    public double UtilizationPercent { get; private set; }
    public DateTime TimestampUtc { get; private set; }

    // EF Core private constructor
    private NetworkInterfaceHistory() { }

    public NetworkInterfaceHistory(
        Guid id,
        Guid tenantId,
        Guid deviceId,
        Guid networkInterfaceId,
        int ifIndex,
        InterfaceAdminStatus adminStatus,
        InterfaceOperStatus operStatus,
        long speedBps,
        long inOctets,
        long outOctets,
        long inErrors,
        long outErrors,
        long inDiscards,
        long outDiscards,
        double utilizationPercent,
        DateTime timestampUtc) : base(id)
    {
        TenantId = tenantId;
        DeviceId = deviceId;
        NetworkInterfaceId = networkInterfaceId;
        IfIndex = ifIndex;
        AdminStatus = adminStatus;
        OperStatus = operStatus;
        SpeedBps = speedBps;
        InOctets = inOctets;
        OutOctets = outOctets;
        InErrors = inErrors;
        OutErrors = outErrors;
        InDiscards = inDiscards;
        OutDiscards = outDiscards;
        UtilizationPercent = utilizationPercent;
        TimestampUtc = timestampUtc;
    }
}