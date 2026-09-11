using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class DiscoveredDeviceCandidate : BaseEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid DiscoveryJobId { get; private set; }
    public DiscoveryJob DiscoveryJob { get; private set; } = null!;

    public string IpAddress { get; private set; } = string.Empty;
    public bool IsIcmpReachable { get; private set; }
    public double? ResponseTimeMs { get; private set; }
    public bool IsSnmpReachable { get; private set; }
    public string? SysDescr { get; private set; }
    public string? SysObjectId { get; private set; }
    public string? SysName { get; private set; }
    public string? MacAddress { get; private set; }

    public DeviceType FingerprintedType { get; private set; } = DeviceType.Unknown;
    public string IdentifiedVendor { get; private set; } = "Unknown";

    public bool IsDuplicate { get; private set; }
    public Guid? ExistingDeviceId { get; private set; }
    public bool IsImported { get; private set; }
    public Guid? ImportedDeviceId { get; private set; }

    // Private constructor for EF Core
    private DiscoveredDeviceCandidate() { }

    public DiscoveredDeviceCandidate(
        Guid id,
        Guid tenantId,
        Guid discoveryJobId,
        string ipAddress,
        bool isIcmpReachable,
        double? responseTimeMs,
        bool isSnmpReachable,
        string? sysDescr,
        string? sysObjectId,
        string? sysName,
        string? macAddress,
        DeviceType fingerprintedType,
        string identifiedVendor,
        bool isDuplicate,
        Guid? existingDeviceId) : base(id)
    {
        TenantId = tenantId;
        DiscoveryJobId = discoveryJobId;
        IpAddress = ipAddress;
        IsIcmpReachable = isIcmpReachable;
        ResponseTimeMs = responseTimeMs;
        IsSnmpReachable = isSnmpReachable;
        SysDescr = sysDescr;
        SysObjectId = sysObjectId;
        SysName = sysName;
        MacAddress = macAddress;
        FingerprintedType = fingerprintedType;
        IdentifiedVendor = identifiedVendor;
        IsDuplicate = isDuplicate;
        ExistingDeviceId = existingDeviceId;
        IsImported = false;
    }

    public void MarkImported(Guid deviceId)
    {
        IsImported = true;
        ImportedDeviceId = deviceId;
    }
}