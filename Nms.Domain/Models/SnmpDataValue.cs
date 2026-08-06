using Nms.Domain.ValueObjects;

namespace Nms.Domain.Models;

public sealed record SnmpDataValue
{
    public Oid Oid { get; }
    public string RawValue { get; }
    public string DataType { get; }

    public SnmpDataValue(Oid oid, string rawValue, string dataType)
    {
        Oid = oid ?? throw new ArgumentNullException(nameof(oid));
        RawValue = rawValue ?? string.Empty;
        DataType = dataType ?? "OctetString";
    }
}