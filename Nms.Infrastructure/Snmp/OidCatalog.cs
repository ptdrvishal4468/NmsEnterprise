using Nms.Application.Common.Interfaces;
using Nms.Domain.ValueObjects;
using Nms.Infrastructure.Telemetry;

namespace Nms.Infrastructure.Snmp;

public sealed class OidCatalog : IOidCatalog
{
    // Standard MIB-II System Group (.1.3.6.1.2.1.1)
    public Oid SystemDescr { get; } = Oid.From(".1.3.6.1.2.1.1.1.0");
    public Oid SystemObjectId { get; } = Oid.From(".1.3.6.1.2.1.1.2.0");
    public Oid SystemUpTime { get; } = Oid.From(".1.3.6.1.2.1.1.3.0");
    public Oid SystemContact { get; } = Oid.From(".1.3.6.1.2.1.1.4.0");
    public Oid SystemName { get; } = Oid.From(".1.3.6.1.2.1.1.5.0");
    public Oid SystemLocation { get; } = Oid.From(".1.3.6.1.2.1.1.6.0");

    // Standard MIB-II Interfaces Group (.1.3.6.1.2.1.2)
    public Oid IfNumber { get; } = Oid.From(".1.3.6.1.2.1.2.1.0");
    public Oid IfTable { get; } = Oid.From(".1.3.6.1.2.1.2.2");

    // Phase 25 Telemetry Additions
    public Oid CiscoCpu5Min { get; } = Oid.From(OidConstants.CiscoCpu5Min);
    public Oid HostMemoryUsed { get; } = Oid.From(OidConstants.HostMemoryUsed);
    public Oid DiskUtilization { get; } = Oid.From(OidConstants.DiskUtilization);
    public Oid Temperature { get; } = Oid.From(OidConstants.CiscoEnvMonTemperature);
    public Oid FanStatus { get; } = Oid.From(OidConstants.CiscoEnvMonFanStatus);
    public Oid PowerSupplyStatus { get; } = Oid.From(OidConstants.CiscoEnvMonSupplyStatus);
}