using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class SnmpTrapMessage : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid? DeviceId { get; set; }
    public SnmpVersion SnmpVersion { get; set; }
    public string? Community { get; set; }
    public string EnterpriseOid { get; set; } = string.Empty;
    public int? GenericTrap { get; set; }
    public int? SpecificTrap { get; set; }
    public string? TrapOid { get; set; }
    public string? AgentAddress { get; set; }
    public string SourceIpAddress { get; set; } = string.Empty;
    public TrapSeverity Severity { get; set; }
    public string VarbindsJson { get; set; } = "[]";
    public string? RawPayloadHex { get; set; }
    public bool IsMalformed { get; set; }
    public DateTime TimestampUtc { get; set; }

    public Device? Device { get; set; }

    public SnmpTrapMessage()
    {
        Id = Guid.NewGuid();
    }

    public SnmpTrapMessage(
        Guid tenantId,
        SnmpVersion snmpVersion,
        string enterpriseOid,
        string sourceIpAddress,
        TrapSeverity severity,
        string varbindsJson,
        DateTime timestampUtc,
        Guid? deviceId = null,
        string? community = null,
        int? genericTrap = null,
        int? specificTrap = null,
        string? trapOid = null,
        string? agentAddress = null,
        string? rawPayloadHex = null,
        bool isMalformed = false) : this()
    {
        TenantId = tenantId;
        DeviceId = deviceId;
        SnmpVersion = snmpVersion;
        Community = community;
        EnterpriseOid = enterpriseOid;
        GenericTrap = genericTrap;
        SpecificTrap = specificTrap;
        TrapOid = trapOid;
        AgentAddress = agentAddress;
        SourceIpAddress = sourceIpAddress;
        Severity = severity;
        VarbindsJson = varbindsJson;
        RawPayloadHex = rawPayloadHex;
        IsMalformed = isMalformed;
        TimestampUtc = timestampUtc;
    }
}