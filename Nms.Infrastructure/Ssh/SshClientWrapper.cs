using System.Diagnostics;
using Renci.SshNet;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Models;
using Nms.Infrastructure.Ssh.Exceptions;
using ISshClient = Nms.Application.Common.Interfaces.ISshClient;

namespace Nms.Infrastructure.Ssh;

public sealed class SshClientWrapper : ISshClient
{
    private readonly ConnectionInfo _connectionInfo;

    public SshClientWrapper(string host, int port, SshCredentials credentials, TimeSpan timeout)
    {
        if (!string.IsNullOrWhiteSpace(credentials.PrivateKey))
        {
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(credentials.PrivateKey));
            var keyFile = string.IsNullOrWhiteSpace(credentials.Passphrase)
                ? new PrivateKeyFile(stream)
                : new PrivateKeyFile(stream, credentials.Passphrase);

            _connectionInfo = new PrivateKeyConnectionInfo(host, port, credentials.Username, keyFile)
            {
                Timeout = timeout
            };
        }
        else
        {
            _connectionInfo = new PasswordConnectionInfo(host, port, credentials.Username, credentials.Password ?? string.Empty)
            {
                Timeout = timeout
            };
        }
    }

    public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            using var client = new Renci.SshNet.SshClient(_connectionInfo);
            try
            {
                client.Connect();
                var isConnected = client.IsConnected;
                client.Disconnect();
                return isConnected;
            }
            catch (Renci.SshNet.Common.SshAuthenticationException ex)
            {
                throw new SshAuthenticationException("SSH Authentication failed.", ex);
            }
            catch (Renci.SshNet.Common.SshConnectionException ex)
            {
                throw new SshTimeoutException($"SSH Connection timed out: {ex.Message}");
            }
            catch (Exception)
            {
                return false;
            }
        }, cancellationToken);
    }

    public async Task<SshExecutionResult> ExecuteCommandAsync(string command, TimeSpan timeout, CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            var stopwatch = Stopwatch.StartNew();
            using var client = new Renci.SshNet.SshClient(_connectionInfo);

            try
            {
                client.Connect();
                using var cmd = client.CreateCommand(command);
                cmd.CommandTimeout = timeout;

                var output = cmd.Execute();
                stopwatch.Stop();

                return SshExecutionResult.Success(output, stopwatch.ElapsedMilliseconds, cmd.ExitStatus ?? 0);
            }
            catch (Renci.SshNet.Common.SshAuthenticationException ex)
            {
                stopwatch.Stop();
                throw new SshAuthenticationException("SSH Authentication failed during command execution.", ex);
            }
            catch (Renci.SshNet.Common.SshOperationTimeoutException)
            {
                stopwatch.Stop();
                return SshExecutionResult.Failure("Command execution timed out.", stopwatch.ElapsedMilliseconds, -1);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return SshExecutionResult.Failure($"SSH Execution Error: {ex.Message}", stopwatch.ElapsedMilliseconds, -1);
            }
            finally
            {
                if (client.IsConnected)
                {
                    client.Disconnect();
                }
            }
        }, cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}