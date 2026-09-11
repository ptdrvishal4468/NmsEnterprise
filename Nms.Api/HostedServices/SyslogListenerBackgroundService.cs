using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Syslog.Commands.IngestSyslogMessage;
using Nms.Infrastructure.Syslog;

namespace Nms.Api.HostedServices;

public class SyslogListenerBackgroundService : BackgroundService
{
    private readonly ISyslogReceiver _syslogReceiver;
    private readonly ISyslogParser _syslogParser;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SyslogListenerBackgroundService> _logger;

    public SyslogListenerBackgroundService(
        ISyslogReceiver syslogReceiver,
        ISyslogParser syslogParser,
        IServiceScopeFactory scopeFactory,
        ILogger<SyslogListenerBackgroundService> logger)
    {
        _syslogReceiver = syslogReceiver;
        _syslogParser = syslogParser;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Syslog Background Listener Service is starting.");

        try
        {
            await _syslogReceiver.StartAsync(stoppingToken);

            if (_syslogReceiver is SyslogUdpReceiver udpReceiver)
            {
                await foreach (var datagram in udpReceiver.Reader.ReadAllAsync(stoppingToken))
                {
                    await ProcessDatagramAsync(datagram, stoppingToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Syslog Background Listener processing loop cancelled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in Syslog Background Listener processing loop.");
        }
    }

    private async Task ProcessDatagramAsync(SyslogDatagram datagram, CancellationToken cancellationToken)
    {
        try
        {
            var parsed = _syslogParser.Parse(datagram.RawMessage, datagram.SourceIpAddress);

            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<ISender>();
            var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();

            var tenantId = tenantContext.IsResolved ? tenantContext.TenantId : Guid.Empty;

            var command = new IngestSyslogMessageCommand(
                TenantId: tenantId,
                SourceIpAddress: datagram.SourceIpAddress,
                Facility: parsed.Facility,
                Severity: parsed.Severity,
                TimestampUtc: parsed.TimestampUtc,
                Hostname: parsed.Hostname,
                AppTag: parsed.AppTag,
                ProcessId: parsed.ProcessId,
                MessageId: parsed.MessageId,
                Message: parsed.Message,
                RawMessage: parsed.RawMessage,
                IsMalformed: parsed.IsMalformed);

            await mediator.Send(command, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ingesting Syslog message from IP {SourceIpAddress}.", datagram.SourceIpAddress);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Syslog Background Listener Service is stopping.");
        await _syslogReceiver.StopAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}