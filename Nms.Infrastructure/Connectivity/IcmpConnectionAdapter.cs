using System.Net.NetworkInformation;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Enums;
using Nms.Domain.Models;

namespace Nms.Infrastructure.Connectivity;

public sealed class IcmpConnectionAdapter : IConnectionAdapter
{
    public NetworkProtocol Protocol => NetworkProtocol.Icmp;

    public async Task<ConnectionResult> TestConnectionAsync(ConnectionParameters parameters, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(parameters.HostOrIp))
        {
            return ConnectionResult.Failure(Protocol, "Host or IP address is required.");
        }

        using var ping = new Ping();
        try
        {
            var timeoutMs = (int)parameters.Timeout.TotalMilliseconds;

            // Register CancellationToken to cancel the ping operation if triggered
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(timeoutMs);

            var reply = await ping.SendPingAsync(parameters.HostOrIp, timeoutMs);

            if (reply.Status == IPStatus.Success)
            {
                return ConnectionResult.Success(Protocol, reply.RoundtripTime);
            }

            return ConnectionResult.Failure(Protocol, $"ICMP Ping failed with status: {reply.Status}");
        }
        catch (OperationCanceledException)
        {
            return ConnectionResult.Failure(Protocol, $"ICMP Ping request timed out after {parameters.Timeout.TotalMilliseconds}ms.");
        }
        catch (Exception ex)
        {
            return ConnectionResult.Failure(Protocol, $"ICMP Ping error: {ex.Message}");
        }
    }
}