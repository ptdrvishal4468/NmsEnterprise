using MediatR;
using Nms.Application.Reachability.Commands.PingDevice;
using Nms.Domain.Interfaces;

namespace Nms.Api.HostedServices;

public class IcmpPollingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IcmpPollingBackgroundService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromMinutes(1);

    public IcmpPollingBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<IcmpPollingBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ICMP Polling Background Service starting.");

        using var timer = new PeriodicTimer(_pollingInterval);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await PerformPollingCycleAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during the ICMP background polling execution.");
            }
        }

        _logger.LogInformation("ICMP Polling Background Service stopping.");
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
                var command = new PingDeviceCommand(device.Id);
                await mediator.Send(command, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to complete ICMP reachability check for device {DeviceId}", device.Id);
            }
        }
    }
}