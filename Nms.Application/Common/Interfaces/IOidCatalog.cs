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
}