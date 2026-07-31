using MediatR;
using Nms.Application.Telemetry.Commands.PollDevice;
using Nms.Application.Telemetry.Commands.ProcessTelemetryData;
using Nms.Domain.Interfaces;

namespace Nms.Api.HostedServices;

public class SnmpPollingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SnmpPollingBackgroundService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromMinutes(1);

    // Standard system OIDs for basic device polling
    private static readonly string[] DefaultOids = new[]
    {
        "1.3.6.1.2.1.25.3.3.1.2", // CPU Load
        "1.3.6.1.2.1.25.2.2.0"     // RAM Usage
    };

    public SnmpPollingBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<SnmpPollingBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SNMP Polling Background Service starting.");

        using var timer = new PeriodicTimer(_pollingInterval);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await PerformPollingCycleAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the SNMP background polling execution.");
            }
        }

        _logger.LogInformation("SNMP Polling Background Service stopping.");
    }

    private async Task PerformPollingCycleAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();
        var deviceRepository = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();

        var devices = await deviceRepository.GetAllAsync(cancellationToken);

        foreach (var device in devices)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                // 1. Trigger Poll
                var pollCommand = new PollDeviceCommand(device, DefaultOids);
                var pollResult = await mediator.Send(pollCommand, cancellationToken);

                if (pollResult.IsSuccess)
                {
                    // 2. Process & Persist Telemetry Data
                    var processCommand = new ProcessTelemetryDataCommand(pollResult);
                    await mediator.Send(processCommand, cancellationToken);
                }
                else
                {
                    _logger.LogWarning("Failed to poll device {DeviceId}: {ErrorMessage}", device.Id, pollResult.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to complete SNMP polling for device {DeviceId}", device.Id);
            }
        }
    }
}