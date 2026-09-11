using System.Diagnostics;
using System.Net;

using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Domain.Entities;

namespace Nms.Infrastructure.Telemetry;

public class SnmpCollectorService : ISnmpCollectorService
{
    private const int DefaultSnmpPort = 161;
    private const int DefaultTimeoutMs = 3000;

    public async Task<SnmpPollResult> PollDeviceAsync(Device device, IEnumerable<string> oids, CancellationToken cancellationToken = default)
    {
        var result = new SnmpPollResult
        {
            DeviceId = device.Id,
            IpAddress = device.IpAddress,
            PolledAtUtc = DateTime.UtcNow
        };

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var ipAddress = IPAddress.Parse(device.IpAddress);
            var endpoint = new IPEndPoint(ipAddress, DefaultSnmpPort);
            var community = new OctetString("public");

            var variableList = oids.Select(oid => new Variable(new ObjectIdentifier(oid))).ToList();

            var response = await Task.Run(() =>
                Messenger.Get(
                    VersionCode.V2,
                    endpoint,
                    community,
                    variableList,
                    DefaultTimeoutMs
                ), cancellationToken);

            stopwatch.Stop();
            result.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            result.IsSuccess = true;

            foreach (var variable in response)
            {
                string key = variable.Id.ToString().TrimStart('.');
                result.OidValues[key] = variable.Data.ToString();
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    public async Task<bool> TestConnectionAsync(Device device, CancellationToken cancellationToken = default)
    {
        var pollResult = await PollDeviceAsync(device, new[] { OidConstants.SystemUptime }, cancellationToken);
        return pollResult.IsSuccess;
    }
}