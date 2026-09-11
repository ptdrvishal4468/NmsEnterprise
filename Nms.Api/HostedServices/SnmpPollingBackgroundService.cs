using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Application.Telemetry.Commands.PollDevice;
using Nms.Application.Telemetry.Commands.ProcessTelemetryData;

namespace Nms.Api.HostedServices;

public class SnmpPollingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IPollingQueue _queue;
    private readonly ILogger<SnmpPollingBackgroundService> _logger;
    private readonly TimeSpan _schedulerInterval = TimeSpan.FromSeconds(30);
    private const int WorkerCount = 4;

    public SnmpPollingBackgroundService(
        IServiceProvider serviceProvider,
        IPollingQueue queue,
        ILogger<SnmpPollingBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SNMP Polling Queue Orchestrator starting with {WorkerCount} concurrent workers.", WorkerCount);

        var workerTasks = new List<Task>();

        // 1. Start Consumer Worker Pool
        for (int i = 0; i < WorkerCount; i++)
        {
            int workerId = i + 1;
            workerTasks.Add(Task.Run(() => ProcessQueueWorkerAsync(workerId, stoppingToken), stoppingToken));
        }

        // 2. Start Producer Scheduling Loop
        workerTasks.Add(Task.Run(() => ScheduleLoopAsync(stoppingToken), stoppingToken));

        await Task.WhenAll(workerTasks);

        _logger.LogInformation("SNMP Polling Queue Orchestrator stopping.");
    }

    private async Task ScheduleLoopAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_schedulerInterval);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var scheduler = scope.ServiceProvider.GetRequiredService<IPollScheduler>();
                await scheduler.SchedulePendingPollsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during scheduled polling evaluation.");
            }
        }
    }

    private async Task ProcessQueueWorkerAsync(int workerId, CancellationToken stoppingToken)
    {
        _logger.LogDebug("Polling Consumer Worker {WorkerId} started.", workerId);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var job = await _queue.DequeueAsync(stoppingToken);
                await ExecutePollJobAsync(job, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Worker {WorkerId} encountered an error processing poll job.", workerId);
            }
        }

        _logger.LogDebug("Polling Consumer Worker {WorkerId} stopped.", workerId);
    }

    private async Task ExecutePollJobAsync(PollJobTask job, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        int attempts = 0;
        bool success = false;

        while (attempts <= job.RetryCount && !success && !cancellationToken.IsCancellationRequested)
        {
            attempts++;
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(job.TimeoutSeconds));

                var pollCommand = new PollDeviceCommand(job.DeviceId);
                var pollResult = await mediator.Send(pollCommand, cts.Token);

                if (pollResult.IsSuccess)
                {
                    var processCommand = new ProcessTelemetryDataCommand(pollResult);
                    await mediator.Send(processCommand, cancellationToken);
                    success = true;
                }
                else
                {
                    _logger.LogWarning("Device {DeviceId} poll attempt {Attempt}/{MaxRetries} failed: {Error}",
                        job.DeviceId, attempts, job.RetryCount + 1, pollResult.ErrorMessage);

                    if (attempts <= job.RetryCount)
                    {
                        await Task.Delay(1000 * attempts, cancellationToken); // Linear backoff
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Device {DeviceId} poll attempt {Attempt} raised an exception.", job.DeviceId, attempts);
            }
        }
    }
}