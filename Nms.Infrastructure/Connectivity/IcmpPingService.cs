using System.Net.NetworkInformation;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Enums;
using Nms.Domain.Models;

namespace Nms.Infrastructure.Connectivity;

public sealed class IcmpPingService : IIcmpPingService
{
    public async Task<PingResult> PingAsync(
        string hostOrIp,
        int count = 4,
        int timeoutMs = 1000,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(hostOrIp))
        {
            return PingResult.CreateFailure(ReachabilityStatus.Offline, count);
        }

        var rtts = new List<long>();
        int packetsReceived = 0;
        bool hasUnreachable = false;

        for (int i = 0; i < count; i++)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            using var ping = new Ping();
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(timeoutMs);

                var reply = await ping.SendPingAsync(hostOrIp, timeoutMs);

                if (reply.Status == IPStatus.Success)
                {
                    packetsReceived++;
                    rtts.Add(reply.RoundtripTime);
                }
                else if (reply.Status == IPStatus.DestinationUnreachable ||
                         reply.Status == IPStatus.DestinationHostUnreachable ||
                         reply.Status == IPStatus.DestinationNetworkUnreachable)
                {
                    hasUnreachable = true;
                }
            }
            catch (OperationCanceledException)
            {
                // Timeout on cancellation/expiry
            }
            catch (PingException)
            {
                // Network or DNS failure
            }

            // Small delay between ping packets to prevent flooding
            if (i < count - 1)
            {
                await Task.Delay(100, cancellationToken);
            }
        }

        if (packetsReceived == 0)
        {
            var failureStatus = hasUnreachable ? ReachabilityStatus.Unreachable : ReachabilityStatus.Timeout;
            return PingResult.CreateFailure(failureStatus, count);
        }

        double minLatency = rtts.Min();
        double maxLatency = rtts.Max();
        double avgLatency = rtts.Average();
        double currentLatency = rtts.Last();
        double packetLossPercentage = ((count - packetsReceived) / (double)count) * 100.0;

        return new PingResult(
            ReachabilityStatus.Online,
            minLatency,
            maxLatency,
            avgLatency,
            currentLatency,
            count,
            packetsReceived,
            packetLossPercentage);
    }
}