using Nms.Application.Common.Interfaces;
using Nms.Domain.ValueObjects;

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
}