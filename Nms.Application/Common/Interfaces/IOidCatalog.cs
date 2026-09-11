using Nms.Domain.ValueObjects;

namespace Nms.Application.Common.Interfaces;

public interface IOidCatalog
{
    Oid SystemDescr { get; }
    Oid SystemObjectId { get; }
    Oid SystemUpTime { get; }
    Oid SystemContact { get; }
    Oid SystemName { get; }
    Oid SystemLocation { get; }
    Oid IfTable { get; }
    Oid IfNumber { get; }

    // Phase 25 Telemetry Additions
    Oid CiscoCpu5Min { get; }
    Oid HostMemoryUsed { get; }
    Oid DiskUtilization { get; }
    Oid Temperature { get; }
    Oid FanStatus { get; }
    Oid PowerSupplyStatus { get; }

    // Phase 28 Interface & Port Management Additions (IF-MIB / RFC 2863)
    Oid IfDescr { get; }
    Oid IfType { get; }
    Oid IfSpeed { get; }
    Oid IfPhysAddress { get; }
    Oid IfAdminStatus { get; }
    Oid IfOperStatus { get; }
    Oid IfInOctets { get; }
    Oid IfOutOctets { get; }
    Oid IfInErrors { get; }
    Oid IfOutErrors { get; }
    Oid IfInDiscards { get; }
    Oid IfOutDiscards { get; }
    Oid IfName { get; }
    Oid IfHCInOctets { get; }
    Oid IfHCOutOctets { get; }
    Oid IfHighSpeed { get; }
}