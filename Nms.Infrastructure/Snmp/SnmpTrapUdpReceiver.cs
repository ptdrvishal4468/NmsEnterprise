using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nms.Application.Common.Interfaces;
using Nms.Infrastructure.Snmp.Options;

namespace Nms.Infrastructure.Snmp;

public sealed record SnmpTrapDatagram(byte[] Data, string SourceIpAddress);

public sealed class SnmpTrapUdpReceiver : ISnmpTrapReceiver, IDisposable
{
    private readonly SnmpTrapOptions _options;
    private readonly ILogger<SnmpTrapUdpReceiver> _logger;
    private readonly Channel<SnmpTrapDatagram> _channel;
    private UdpClient? _udpClient;
    private CancellationTokenSource? _cts;
    private Task? _listenTask;
    private bool _disposed;

    public ChannelReader<SnmpTrapDatagram> Reader => _channel.Reader;

    public SnmpTrapUdpReceiver(
        IOptions<SnmpTrapOptions> options,
        ILogger<SnmpTrapUdpReceiver> logger)
    {
        _options = options.Value;
        _logger = logger;

        var channelOptions = new BoundedChannelOptions(_options.BufferSize)
        {
            SingleWriter = true,
            SingleReader = false,
            FullMode = BoundedChannelFullMode.DropOldest
        };

        _channel = Channel.CreateBounded<SnmpTrapDatagram>(channelOptions);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("SNMP Trap UDP Receiver is disabled by configuration.");
            return Task.CompletedTask;
        }

        _cts = new CancellationTokenSource();

        try
        {
            _udpClient = new UdpClient();
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, _options.Port));

            _logger.LogInformation("SNMP Trap UDP Receiver started listening on port {Port}.", _options.Port);
            _listenTask = ListenAsync(_cts.Token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to bind SNMP Trap UDP Receiver on port {Port}.", _options.Port);
        }

        return Task.CompletedTask;
    }

    private async Task ListenAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var result = await _udpClient!.ReceiveAsync(cancellationToken);
                var datagram = new SnmpTrapDatagram(result.Buffer, result.RemoteEndPoint.Address.ToString());

                await _channel.Writer.WriteAsync(datagram, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while receiving SNMP Trap UDP datagram.");
            }
        }

        _channel.Writer.TryComplete();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_cts != null)
        {
            await _cts.CancelAsync();
        }

        _udpClient?.Close();

        if (_listenTask != null)
        {
            try
            {
                await _listenTask;
            }
            catch (OperationCanceledException)
            {
                // Expected on cancellation shutdown
            }
        }

        _logger.LogInformation("SNMP Trap UDP Receiver stopped.");
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _cts?.Dispose();
        _udpClient?.Dispose();
    }
}