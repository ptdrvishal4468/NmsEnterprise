using System.Diagnostics;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Models;

namespace Nms.Infrastructure.Connectivity;

public sealed class SnmpConnectionAdapter : IConnectionAdapter
{
    private readonly ISnmpCollectorService _snmpCollectorService;

    public SnmpConnectionAdapter(ISnmpCollectorService snmpCollectorService)
    {
        _snmpCollectorService = snmpCollectorService ?? throw new ArgumentNullException(nameof(snmpCollectorService));
    }

    public NetworkProtocol Protocol => NetworkProtocol.SnmpV2c;

    public async Task<ConnectionResult> TestConnectionAsync(ConnectionParameters parameters, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(parameters.HostOrIp))
        {
            return ConnectionResult.Failure(Protocol, "Host or IP address is required.");
        }

        var stopwatch = Stopwatch.StartNew();
        try
        {
            // Instantiating transient Device entity via its public constructor
            var transientDevice = new Device(
                id: Guid.NewGuid(),
                tenantId: Guid.Empty,
                name: "Transient-Connectivity-Test-Device",
                ipAddress: parameters.HostOrIp,
                deviceType: DeviceType.Unknown,
                snmpPort: parameters.Port > 0 ? parameters.Port : 161
            );

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(parameters.Timeout);

            var isConnected = await _snmpCollectorService.TestConnectionAsync(transientDevice, cts.Token);

            stopwatch.Stop();

            if (isConnected)
            {
                return ConnectionResult.Success(Protocol, stopwatch.ElapsedMilliseconds);
            }

            return ConnectionResult.Failure(Protocol, "SNMP agent did not respond or connection timed out.");
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            return ConnectionResult.Failure(Protocol, $"SNMP check timed out after {parameters.Timeout.TotalMilliseconds}ms.");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return ConnectionResult.Failure(Protocol, $"SNMP transport error: {ex.Message}");
        }
    }
}