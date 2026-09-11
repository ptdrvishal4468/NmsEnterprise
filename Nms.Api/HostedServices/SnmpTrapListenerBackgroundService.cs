using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Nms.Application.Common.Interfaces;
using Nms.Application.SnmpTraps.Commands.IngestSnmpTrap;
using Nms.Infrastructure.Snmp;

namespace Nms.Api.HostedServices;

public sealed class SnmpTrapListenerBackgroundService : BackgroundService
{
    private readonly ISnmpTrapReceiver _receiver;
    private readonly ISnmpTrapParser _parser;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SnmpTrapListenerBackgroundService> _logger;

    public SnmpTrapListenerBackgroundService(
        ISnmpTrapReceiver receiver,
        ISnmpTrapParser parser,
        IServiceScopeFactory scopeFactory,
        ILogger<SnmpTrapListenerBackgroundService> logger)
    {
        _receiver = receiver;
        _parser = parser;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting SNMP Trap Listener Background Service...");
        await _receiver.StartAsync(stoppingToken);

        if (_receiver is SnmpTrapUdpReceiver udpReceiver)
        {
            try
            {
                await foreach (var datagram in udpReceiver.Reader.ReadAllAsync(stoppingToken))
                {
                    try
                    {
                        var parsedTrap = _parser.Parse(datagram.Data, datagram.SourceIpAddress);

                        using var scope = _scopeFactory.CreateScope();
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                        var command = new IngestSnmpTrapCommand(
                            Version: parsedTrap.Version,
                            EnterpriseOid: parsedTrap.EnterpriseOid,
                            SourceIpAddress: parsedTrap.SourceIpAddress,
                            Severity: parsedTrap.Severity,
                            VarbindsJson: parsedTrap.VarbindsJson,
                            TimestampUtc: parsedTrap.TimestampUtc,
                            Community: parsedTrap.Community,
                            GenericTrap: parsedTrap.GenericTrap,
                            SpecificTrap: parsedTrap.SpecificTrap,
                            TrapOid: parsedTrap.TrapOid,
                            AgentAddress: parsedTrap.AgentAddress,
                            RawPayloadHex: parsedTrap.RawPayloadHex,
                            IsMalformed: parsedTrap.IsMalformed);

                        await mediator.Send(command, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing SNMP Trap datagram from source IP {SourceIp}.", datagram.SourceIpAddress);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected on service shutdown
            }
        }

        await _receiver.StopAsync(stoppingToken);
        _logger.LogInformation("SNMP Trap Listener Background Service stopped.");
    }
}