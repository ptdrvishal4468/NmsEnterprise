using MediatR;
using Nms.Domain.Enums;

namespace Nms.Application.SnmpTraps.Commands.IngestSnmpTrap;

public sealed record IngestSnmpTrapCommand(
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
    bool IsMalformed = false) : IRequest<Guid>;