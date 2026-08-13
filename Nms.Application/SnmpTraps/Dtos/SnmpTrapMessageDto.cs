using Nms.Domain.Enums;

namespace Nms.Application.SnmpTraps.Dtos;

public sealed record SnmpTrapMessageDto(
    Guid Id,
    Guid TenantId,
    Guid? DeviceId,
    SnmpVersion SnmpVersion,
    string? Community,
    string EnterpriseOid,
    int? GenericTrap,
    int? SpecificTrap,
    string? TrapOid,
    string? AgentAddress,
    string SourceIpAddress,
    TrapSeverity Severity,
    string VarbindsJson,
    string? RawPayloadHex,
    bool IsMalformed,
    DateTime TimestampUtc,
    DateTime CreatedAtUtc);