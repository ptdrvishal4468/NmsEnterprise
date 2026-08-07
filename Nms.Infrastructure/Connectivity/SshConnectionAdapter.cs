using System.Diagnostics;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Enums;
using Nms.Domain.Models;

namespace Nms.Infrastructure.Connectivity;

public sealed class SshConnectionAdapter : IConnectionAdapter
{
    private readonly ISshClientFactory _sshClientFactory;

    public NetworkProtocol Protocol => NetworkProtocol.Ssh;

    public SshConnectionAdapter(ISshClientFactory sshClientFactory)
    {
        _sshClientFactory = sshClientFactory ?? throw new ArgumentNullException(nameof(sshClientFactory));
    }

    public async Task<ConnectionResult> TestConnectionAsync(ConnectionParameters parameters, CancellationToken cancellationToken)
    {
        if (parameters.Protocol != NetworkProtocol.Ssh)
        {
            return ConnectionResult.Failure(Protocol, $"Invalid protocol for SshConnectionAdapter: {parameters.Protocol}");
        }

        if (string.IsNullOrWhiteSpace(parameters.Username))
        {
            return ConnectionResult.Failure(Protocol, "Username is required for SSH connection testing.");
        }

        try
        {
            var credentials = SshCredentials.FromPassword(parameters.Username, parameters.Password ?? string.Empty);
            var stopwatch = Stopwatch.StartNew();

            await using var client = _sshClientFactory.CreateClient(
                parameters.HostOrIp,
                parameters.Port > 0 ? parameters.Port : 22,
                credentials,
                parameters.Timeout);

            var isConnected = await client.TestConnectionAsync(cancellationToken);
            stopwatch.Stop();

            return isConnected
                ? ConnectionResult.Success(Protocol, stopwatch.ElapsedMilliseconds)
                : ConnectionResult.Failure(Protocol, "Failed to establish SSH session.");
        }
        catch (Exception ex)
        {
            return ConnectionResult.Failure(Protocol, $"SSH Connection error: {ex.Message}");
        }
    }
}