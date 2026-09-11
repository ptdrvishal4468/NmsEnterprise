using MediatR;
using Nms.Domain.Enums;

namespace Nms.Application.Syslog.Commands.IngestSyslogMessage;

public record IngestSyslogMessageCommand(
    Guid TenantId,
    string SourceIpAddress,
    SyslogFacility Facility,
    SyslogSeverity Severity,
    DateTime TimestampUtc,
    string? Hostname,
    string? AppTag,
    string? ProcessId,
    string? MessageId,
    string Message,
    string RawMessage,
    bool IsMalformed) : IRequest<Guid>;