using Microsoft.Extensions.Logging;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Notifications.Services;

public class NotificationDispatcher : INotificationDispatcher
{
    private readonly IEnumerable<INotificationProvider> _providers;
    private readonly INotificationTemplateRepository _templateRepository;
    private readonly INotificationLogRepository _logRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly INotificationTemplateEngine _templateEngine;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NotificationDispatcher> _logger;

    public NotificationDispatcher(
        IEnumerable<INotificationProvider> providers,
        INotificationTemplateRepository templateRepository,
        INotificationLogRepository logRepository,
        IDeviceRepository deviceRepository,
        INotificationTemplateEngine templateEngine,
        IUnitOfWork unitOfWork,
        ILogger<NotificationDispatcher> logger)
    {
        _providers = providers;
        _templateRepository = templateRepository;
        _logRepository = logRepository;
        _deviceRepository = deviceRepository;
        _templateEngine = templateEngine;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task DispatchAlertNotificationAsync(Alert alert, CancellationToken cancellationToken = default)
    {
        var templates = await _templateRepository.GetActiveTemplatesForTenantAsync(alert.TenantId, cancellationToken);
        if (!templates.Any())
        {
            _logger.LogInformation("No active notification templates found for Tenant {TenantId}", alert.TenantId);
            return;
        }

        var device = await _deviceRepository.GetByIdAsync(alert.DeviceId, cancellationToken);

        foreach (var template in templates)
        {
            var provider = _providers.FirstOrDefault(p => p.Channel == template.Channel);
            if (provider == null)
            {
                _logger.LogWarning("No registered provider adapter found for channel {Channel}", template.Channel);
                continue;
            }

            string renderedSubject = _templateEngine.Render(template.SubjectTemplate, alert, device);
            string renderedBody = _templateEngine.Render(template.BodyTemplate, alert, device);

            var log = new NotificationLog(
                alert.TenantId,
                alert.Id,
                template.Channel,
                template.RecipientTarget,
                renderedSubject,
                renderedBody);

            await _logRepository.AddAsync(log, cancellationToken);

            try
            {
                bool success = await provider.SendAsync(template.RecipientTarget, renderedSubject, renderedBody, cancellationToken);
                if (success)
                {
                    log.MarkAsSent();
                }
                else
                {
                    log.MarkAsFailed("Provider returned failure during transmission.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification via {Channel} to {Target}", template.Channel, template.RecipientTarget);
                log.MarkAsFailed(ex.Message);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}