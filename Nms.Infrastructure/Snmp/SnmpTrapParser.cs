using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Lextm.SharpSnmpLib.Security;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Enums;
using System.Text.Json;

namespace Nms.Infrastructure.Snmp;

public sealed class SnmpTrapParser : ISnmpTrapParser
{
    public ParsedSnmpTrap Parse(byte[] payload, string sourceIpAddress)
    {
        try
        {
            var messages = MessageFactory.ParseMessages(payload, 0, payload.Length, new UserRegistry());
            if (messages == null || messages.Count == 0)
            {
                return CreateMalformedResult(sourceIpAddress, payload);
            }

            var message = messages[0];
            var version = message.Version switch
            {
                VersionCode.V1 => SnmpVersion.V1,
                VersionCode.V2 => SnmpVersion.V2c,
                VersionCode.V3 => SnmpVersion.V3,
                _ => SnmpVersion.V2c
            };

            if (message is TrapV1Message v1Msg)
            {
                var community = v1Msg.Community().ToString();
                var enterpriseOid = v1Msg.Enterprise.ToString();
                var agentAddr = v1Msg.AgentAddress.ToString();
                var genericTrap = (int)v1Msg.Generic;
                var specificTrap = v1Msg.Specific;

                var varbindList = v1Msg.Variables().Select(v => new
                {
                    Oid = v.Id.ToString(),
                    Value = v.Data.ToString(),
                    Type = v.Data.TypeCode.ToString()
                }).ToList();

                var varbindsJson = JsonSerializer.Serialize(varbindList);
                var severity = MapV1TrapSeverity(genericTrap);

                return new ParsedSnmpTrap(
                    Version: version,
                    EnterpriseOid: enterpriseOid,
                    SourceIpAddress: sourceIpAddress,
                    Severity: severity,
                    VarbindsJson: varbindsJson,
                    TimestampUtc: DateTime.UtcNow,
                    Community: community,
                    GenericTrap: genericTrap,
                    SpecificTrap: specificTrap,
                    AgentAddress: agentAddr,
                    RawPayloadHex: Convert.ToHexString(payload),
                    IsMalformed: false);
            }

            if (message is TrapV2Message v2Msg)
            {
                var community = v2Msg.Community().ToString();
                var varbinds = v2Msg.Variables();

                string trapOid = string.Empty;
                string enterpriseOid = ".1.3.6.1.4.1";

                var varbindList = varbinds.Select(v => new
                {
                    Oid = v.Id.ToString(),
                    Value = v.Data.ToString(),
                    Type = v.Data.TypeCode.ToString()
                }).ToList();

                var trapOidVar = varbinds.FirstOrDefault(v =>
                    v.Id.ToString() == "1.3.6.1.6.3.1.1.4.1.0" ||
                    v.Id.ToString() == ".1.3.6.1.6.3.1.1.4.1.0");

                if (trapOidVar != null)
                {
                    trapOid = trapOidVar.Data.ToString();
                    enterpriseOid = trapOid;
                }

                var varbindsJson = JsonSerializer.Serialize(varbindList);
                var severity = MapOidSeverity(trapOid);

                return new ParsedSnmpTrap(
                    Version: version,
                    EnterpriseOid: enterpriseOid,
                    SourceIpAddress: sourceIpAddress,
                    Severity: severity,
                    VarbindsJson: varbindsJson,
                    TimestampUtc: DateTime.UtcNow,
                    Community: community,
                    TrapOid: trapOid,
                    RawPayloadHex: Convert.ToHexString(payload),
                    IsMalformed: false);
            }

            return CreateMalformedResult(sourceIpAddress, payload);
        }
        catch
        {
            return CreateMalformedResult(sourceIpAddress, payload);
        }
    }

    private static TrapSeverity MapV1TrapSeverity(int genericTrap)
    {
        return genericTrap switch
        {
            2 => TrapSeverity.Error,         // linkDown
            4 => TrapSeverity.Warning,       // authenticationFailure
            5 => TrapSeverity.Error,         // egpNeighborLoss
            0 => TrapSeverity.Warning,       // coldStart
            1 => TrapSeverity.Informational, // warmStart
            3 => TrapSeverity.Informational, // linkUp
            _ => TrapSeverity.Informational
        };
    }

    private static TrapSeverity MapOidSeverity(string trapOid)
    {
        if (trapOid.Contains("fail", StringComparison.OrdinalIgnoreCase) ||
            trapOid.Contains("down", StringComparison.OrdinalIgnoreCase) ||
            trapOid.Contains("error", StringComparison.OrdinalIgnoreCase))
        {
            return TrapSeverity.Error;
        }

        if (trapOid.Contains("warn", StringComparison.OrdinalIgnoreCase) ||
            trapOid.Contains("degraded", StringComparison.OrdinalIgnoreCase))
        {
            return TrapSeverity.Warning;
        }

        return TrapSeverity.Informational;
    }

    private static ParsedSnmpTrap CreateMalformedResult(string sourceIpAddress, byte[] payload)
    {
        return new ParsedSnmpTrap(
            Version: SnmpVersion.V2c,
            EnterpriseOid: "UNKNOWN",
            SourceIpAddress: sourceIpAddress,
            Severity: TrapSeverity.Warning,
            VarbindsJson: "[]",
            TimestampUtc: DateTime.UtcNow,
            RawPayloadHex: Convert.ToHexString(payload),
            IsMalformed: true);
    }
}