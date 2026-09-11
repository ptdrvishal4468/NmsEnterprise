using Nms.Domain.Enums;

namespace Nms.Application.Common.Interfaces;

public sealed record ParsedSnmpTrap(
    SnmpVersion Version,
    string EnterpriseOid,
    string SourceIpAddress,
    TrapSeverity Severity,
    string VarbindsJson,
    DateTime TimestampUtc,
    string? Community = null,
    int? GenericTrap = null,
    int? SpecificTrap = null,
    string? TrapOid = null,
    string? AgentAddress = null,
    string? RawPayloadHex = null,
    bool IsMalformed = false);

public interface ISnmpTrapParser
{
    ParsedSnmpTrap Parse(byte[] payload, string sourceIpAddress);
}