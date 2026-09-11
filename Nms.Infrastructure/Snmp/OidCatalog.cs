using Nms.Application.Common.Interfaces;
using Nms.Domain.ValueObjects;

namespace Nms.Infrastructure.Snmp;

public class OidCatalog : IOidCatalog
{
    public Oid SystemDescr => new("1.3.6.1.2.1.1.1.0");
    public Oid SystemObjectId => new("1.3.6.1.2.1.1.2.0");
    public Oid SystemUpTime => new("1.3.6.1.2.1.1.3.0");
    public Oid SystemContact => new("1.3.6.1.2.1.1.4.0");
    public Oid SystemName => new("1.3.6.1.2.1.1.5.0");
    public Oid SystemLocation => new("1.3.6.1.2.1.1.6.0");
    public Oid IfTable => new("1.3.6.1.2.1.2.2");
    public Oid IfNumber => new("1.3.6.1.2.1.2.1.0");

    // Telemetry
    public Oid CiscoCpu5Min => new("1.3.6.1.4.1.9.9.109.1.1.1.1.5");
    public Oid HostMemoryUsed => new("1.3.6.1.2.1.25.2.3.1.6");
    public Oid DiskUtilization => new("1.3.6.1.2.1.25.2.3.1.5");
    public Oid Temperature => new("1.3.6.1.4.1.9.9.13.1.3.1.3");
    public Oid FanStatus => new("1.3.6.1.4.1.9.9.13.1.4.1.3");
    public Oid PowerSupplyStatus => new("1.3.6.1.4.1.9.9.13.1.5.1.3");

    // Interface & Port Management (IF-MIB / MIB-II)
    public Oid IfDescr => new("1.3.6.1.2.1.2.2.1.2");
    public Oid IfType => new("1.3.6.1.2.1.2.2.1.3");
    public Oid IfSpeed => new("1.3.6.1.2.1.2.2.1.5");
    public Oid IfPhysAddress => new("1.3.6.1.2.1.2.2.1.6");
    public Oid IfAdminStatus => new("1.3.6.1.2.1.2.2.1.7");
    public Oid IfOperStatus => new("1.3.6.1.2.1.2.2.1.8");
    public Oid IfInOctets => new("1.3.6.1.2.1.2.2.1.10");
    public Oid IfInDiscards => new("1.3.6.1.2.1.2.2.1.11");
    public Oid IfInErrors => new("1.3.6.1.2.1.2.2.1.12");
    public Oid IfOutOctets => new("1.3.6.1.2.1.2.2.1.16");
    public Oid IfOutDiscards => new("1.3.6.1.2.1.2.2.1.17");
    public Oid IfOutErrors => new("1.3.6.1.2.1.2.2.1.18");

    // ifXTable (High Capacity 64-bit Counters)
    public Oid IfName => new("1.3.6.1.2.1.31.1.1.1.1");
    public Oid IfHCInOctets => new("1.3.6.1.2.1.31.1.1.1.6");
    public Oid IfHCOutOctets => new("1.3.6.1.2.1.31.1.1.1.10");
    public Oid IfHighSpeed => new("1.3.6.1.2.1.31.1.1.1.15");
}