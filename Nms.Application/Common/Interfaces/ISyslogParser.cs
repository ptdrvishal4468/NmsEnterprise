using Nms.Domain.Enums;

namespace Nms.Application.Common.Interfaces;

public record ParsedSyslogMessage(
    SyslogFacility Facility,
    SyslogSeverity Severity,
    DateTime TimestampUtc,
    string? Hostname,
    string? AppTag,
    string? ProcessId,
    string? MessageId,
    string Message,
    string RawMessage,
    bool IsMalformed);

public interface ISyslogParser
{
    ParsedSyslogMessage Parse(string rawMessage, string sourceIpAddress);
}