using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nms.Application.Common.Interfaces;
using Nms.Infrastructure.Syslog.Options;

namespace Nms.Infrastructure.Syslog;

public record SyslogDatagram(string RawMessage, string SourceIpAddress, DateTime ReceivedAtUtc);

public class SyslogUdpReceiver : ISyslogReceiver
{
    private readonly SyslogOptions _options;
    private readonly ILogger<SyslogUdpReceiver> _logger;
    private readonly Channel<SyslogDatagram> _channel;
    private UdpClient? _udpClient;
    private CancellationTokenSource? _cts;

    public ChannelReader<SyslogDatagram> Reader => _channel.Reader;

    public SyslogUdpReceiver(
        IOptions<SyslogOptions> options,
        ILogger<SyslogUdpReceiver> logger)
    {
        _options = options.Value;
        _logger = logger;

        var channelOptions = new BoundedChannelOptions(_options.BufferSize)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleWriter = true,
            SingleReader = false
        };
        _channel = Channel.CreateBounded<SyslogDatagram>(channelOptions);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("Syslog UDP Receiver is disabled by configuration.");
            return Task.CompletedTask;
        }

        try
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Configure socket reuse address prior to binding
            _udpClient = new UdpClient();
            _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, _options.Port));

            _logger.LogInformation("Syslog UDP Listener successfully bound to port {Port}.", _options.Port);

            _ = ListenLoopAsync(_cts.Token);
        }
        catch (SocketException ex)
        {
            _logger.LogWarning(ex, "Failed to bind Syslog UDP Listener to port {Port}. Listener will be disabled for this instance.", _options.Port);
        }

        return Task.CompletedTask;
    }

    private async Task ListenLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _udpClient != null)
        {
            try
            {
                var result = await _udpClient.ReceiveAsync(cancellationToken);
                var rawText = Encoding.UTF8.GetString(result.Buffer);
                var datagram = new SyslogDatagram(rawText, result.RemoteEndPoint.Address.ToString(), DateTime.UtcNow);

                _channel.Writer.TryWrite(datagram);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error receiving Syslog UDP packet.");
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts?.Cancel();
        _udpClient?.Close();
        _udpClient?.Dispose();
        _channel.Writer.TryComplete();
        _logger.LogInformation("Syslog UDP Listener stopped.");
        return Task.CompletedTask;
    }
}