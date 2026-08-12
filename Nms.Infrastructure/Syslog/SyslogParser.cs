using System.Globalization;
using System.Text.RegularExpressions;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Enums;

namespace Nms.Infrastructure.Syslog;

public class SyslogParser : ISyslogParser
{
    // RFC 3164 Regex: <PRI>MMM DD HH:MM:SS HOSTNAME TAG: MSG
    private static readonly Regex Rfc3164Regex = new(
    @"^<(?<pri>\d{1,3})>(?<time>[A-Za-z]{3}\s+\d{1,2}\s+\d{2}:\d{2}:\d{2})\s+(?<host>\S+)\s+(?<tag>[^:\[\s]+)(?:\[(?<pid>\d+)\])?:\s*(?<msg>.*)$",
    RegexOptions.Compiled | RegexOptions.Singleline);

    // RFC 5424 Regex: <PRI>VERSION TIMESTAMP HOSTNAME APP-NAME PROCID MSGID STRUCTURED-DATA MSG
    private static readonly Regex Rfc5424Regex = new(
        @"^<(?<pri>\d{1,3})>1\s+(?<time>\S+)\s+(?<host>\S+)\s+(?<app>\S+)\s+(?<procid>\S+)\s+(?<msgid>\S+)\s+(?<sd>\[.*?\]|-)\s*(?<msg>.*)$",
        RegexOptions.Compiled | RegexOptions.Singleline);

    public ParsedSyslogMessage Parse(string rawMessage, string sourceIpAddress)
    {
        if (string.IsNullOrWhiteSpace(rawMessage))
        {
            return CreateMalformed(rawMessage, "Empty message payload");
        }

        var trimmed = rawMessage.Trim();

        // 1. Try RFC 5424 (IETF Standard)
        var match5424 = Rfc5424Regex.Match(trimmed);
        if (match5424.Success)
        {
            return ParseRfc5424(match5424, trimmed);
        }

        // 2. Try RFC 3164 (BSD Standard)
        var match3164 = Rfc3164Regex.Match(trimmed);
        if (match3164.Success)
        {
            return ParseRfc3164(match3164, trimmed);
        }

        // 3. Simple PRI Header Fallback Parse
        if (trimmed.StartsWith('<') && trimmed.Contains('>'))
        {
            return ParseFallbackPri(trimmed);
        }

        // 4. Complete Fallback for Unstructured Text
        return CreateMalformed(trimmed, trimmed);
    }

    private static ParsedSyslogMessage ParseRfc5424(Match match, string raw)
    {
        int pri = int.Parse(match.Groups["pri"].Value);
        var (facility, severity) = CalculateFacilityAndSeverity(pri);

        var timeStr = match.Groups["time"].Value;
        DateTime timestamp = DateTime.TryParse(timeStr, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dt)
            ? dt
            : DateTime.UtcNow;

        string host = NilOrValue(match.Groups["host"].Value);
        string app = NilOrValue(match.Groups["app"].Value);
        string procId = NilOrValue(match.Groups["procid"].Value);
        string msgId = NilOrValue(match.Groups["msgid"].Value);
        string msg = match.Groups["msg"].Value;

        return new ParsedSyslogMessage(
            Facility: facility,
            Severity: severity,
            TimestampUtc: timestamp,
            Hostname: host,
            AppTag: app,
            ProcessId: procId,
            MessageId: msgId,
            Message: string.IsNullOrWhiteSpace(msg) ? raw : msg,
            RawMessage: raw,
            IsMalformed: false);
    }

    private static ParsedSyslogMessage ParseRfc3164(Match match, string raw)
    {
        int pri = int.Parse(match.Groups["pri"].Value);
        var (facility, severity) = CalculateFacilityAndSeverity(pri);

        var timeStr = match.Groups["time"].Value;
        // RFC 3164 timestamps omit the year, default to current year UTC
        var currentYearStr = $"{DateTime.UtcNow.Year} {timeStr}";
        DateTime timestamp = DateTime.TryParseExact(currentYearStr, "yyyy MMM d HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dt)
            ? dt
            : DateTime.UtcNow;

        string host = match.Groups["host"].Value;
        string tag = match.Groups["tag"].Value;
        string? pid = match.Groups["pid"].Success ? match.Groups["pid"].Value : null;
        string msg = match.Groups["msg"].Value;

        return new ParsedSyslogMessage(
            Facility: facility,
            Severity: severity,
            TimestampUtc: timestamp,
            Hostname: host,
            AppTag: tag,
            ProcessId: pid,
            MessageId: null,
            Message: string.IsNullOrWhiteSpace(msg) ? raw : msg,
            RawMessage: raw,
            IsMalformed: false);
    }

    private static ParsedSyslogMessage ParseFallbackPri(string trimmed)
    {
        int closingBracket = trimmed.IndexOf('>');
        if (closingBracket > 1 && int.TryParse(trimmed.Substring(1, closingBracket - 1), out int pri))
        {
            var (facility, severity) = CalculateFacilityAndSeverity(pri);
            string body = trimmed[(closingBracket + 1)..].Trim();

            return new ParsedSyslogMessage(
                Facility: facility,
                Severity: severity,
                TimestampUtc: DateTime.UtcNow,
                Hostname: null,
                AppTag: null,
                ProcessId: null,
                MessageId: null,
                Message: body,
                RawMessage: trimmed,
                IsMalformed: true);
        }

        return CreateMalformed(trimmed, trimmed);
    }

    private static ParsedSyslogMessage CreateMalformed(string raw, string messageBody)
    {
        return new ParsedSyslogMessage(
            Facility: SyslogFacility.Local7,
            Severity: SyslogSeverity.Notice,
            TimestampUtc: DateTime.UtcNow,
            Hostname: null,
            AppTag: "Malformed",
            ProcessId: null,
            MessageId: null,
            Message: messageBody,
            RawMessage: raw,
            IsMalformed: true);
    }

    private static (SyslogFacility Facility, SyslogSeverity Severity) CalculateFacilityAndSeverity(int pri)
    {
        int facilityNum = pri / 8;
        int severityNum = pri % 8;

        var facility = Enum.IsDefined(typeof(SyslogFacility), facilityNum)
            ? (SyslogFacility)facilityNum
            : SyslogFacility.Local7;

        var severity = Enum.IsDefined(typeof(SyslogSeverity), severityNum)
            ? (SyslogSeverity)severityNum
            : SyslogSeverity.Notice;

        return (facility, severity);
    }

    private static string NilOrValue(string val) => val == "-" ? string.Empty : val;
}