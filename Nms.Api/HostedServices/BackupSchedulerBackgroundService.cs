using Nms.Application.Common.Interfaces;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Api.HostedServices;

public class BackupSchedulerBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BackupSchedulerBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

    public BackupSchedulerBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<BackupSchedulerBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Backup Scheduler Background Service started.");

        using var timer = new PeriodicTimer(_checkInterval);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ProcessDueSchedulesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing backup schedules.");
            }
        }
    }

    private async Task ProcessDueSchedulesAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var backupEngine = scope.ServiceProvider.GetRequiredService<IConfigurationBackupEngine>();

        var dueSchedules = await unitOfWork.BackupSchedules.GetDueSchedulesAsync(cancellationToken);

        if (dueSchedules.Count == 0)
            return;

        _logger.LogInformation("Found {Count} backup schedule(s) due for execution.", dueSchedules.Count);

        foreach (var schedule in dueSchedules)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                if (schedule.DeviceId.HasValue)
                {
                    await backupEngine.ExecuteBackupAsync(
                        schedule.TenantId,
                        schedule.DeviceId.Value,
                        BackupTriggerType.Scheduled,
                        cancellationToken);
                }
                else
                {
                    // Tenant-wide schedule for all devices
                    var (devices, _) = await unitOfWork.Devices.GetPagedAsync(
                        pageIndex: 1,
                        pageSize: 500,
                        searchTerm: null,
                        deviceType: null,
                        status: null,
                        cancellationToken: cancellationToken);

                    var tenantDevices = devices.Where(d => d.TenantId == schedule.TenantId).ToList();

                    foreach (var device in tenantDevices)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        await backupEngine.ExecuteBackupAsync(
                            schedule.TenantId,
                            device.Id,
                            BackupTriggerType.Scheduled,
                            cancellationToken);
                    }
                }

                schedule.RecordExecution();
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute backup schedule '{ScheduleId}' for tenant '{TenantId}'.", schedule.Id, schedule.TenantId);
            }
        }
    }
}