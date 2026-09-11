using MediatR;
using Nms.Application.Reporting.Commands.ExecuteScheduledReport;
using Nms.Domain.Interfaces;

namespace Nms.Api.HostedServices;

public class ReportSchedulerBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReportSchedulerBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

    public ReportSchedulerBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<ReportSchedulerBackgroundService> logger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Report Scheduler Background Service started.");

        using var timer = new PeriodicTimer(_checkInterval);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ProcessDueReportsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing scheduled reports.");
            }
        }
    }

    private async Task ProcessDueReportsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var dueReports = await unitOfWork.ScheduledReports.GetDueReportsAsync(DateTime.UtcNow, cancellationToken);

        if (dueReports.Count == 0)
            return;

        _logger.LogInformation("Found {Count} scheduled report(s) due for execution.", dueReports.Count);

        foreach (var report in dueReports)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                await sender.Send(new ExecuteScheduledReportCommand(report.Id), cancellationToken);
                _logger.LogInformation("Successfully executed scheduled report '{ReportId}' ('{ReportName}') for tenant '{TenantId}'.", report.Id, report.Name, report.TenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute scheduled report '{ReportId}' for tenant '{TenantId}'.", report.Id, report.TenantId);
            }
        }
    }
}