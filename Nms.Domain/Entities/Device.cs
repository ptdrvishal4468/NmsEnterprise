using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class Device : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public string Name { get; private set; } = string.Empty;
    public string IpAddress { get; private set; } = string.Empty;
    public DeviceType DeviceType { get; private set; } = DeviceType.Unknown;
    public int SnmpPort { get; private set; } = 161;
    public string? SnmpV3User { get; private set; }
    public string? SnmpV3AuthKeyEncrypted { get; private set; }
    public string? SnmpV3PrivKeyEncrypted { get; private set; }
    public DeviceStatus Status { get; private set; } = DeviceStatus.Unknown;
    public DateTime? LastSeenUtc { get; private set; }

    // EF Core private constructor
    private Device() { }

    public Device(
        Guid id,
        Guid tenantId,
        string name,
        string ipAddress,
        DeviceType deviceType,
        int snmpPort = 161) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Device name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("Device IP address is required.", nameof(ipAddress));

        TenantId = tenantId;
        Name = name;
        IpAddress = ipAddress;
        DeviceType = deviceType;
        SnmpPort = snmpPort;
        Status = DeviceStatus.Unknown;
    }

    public void UpdateStatus(DeviceStatus newStatus)
    {
        Status = newStatus;
        if (newStatus == DeviceStatus.Online)
        {
            LastSeenUtc = DateTime.UtcNow;
        }
    }

    public void ConfigureSnmpV3Credentials(string username, string encryptedAuthKey, string encryptedPrivKey)
    {
        SnmpV3User = username;
        SnmpV3AuthKeyEncrypted = encryptedAuthKey;
        SnmpV3PrivKeyEncrypted = encryptedPrivKey;
    }
}