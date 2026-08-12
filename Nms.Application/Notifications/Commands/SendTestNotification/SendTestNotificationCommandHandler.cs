using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Notifications.Commands.SendTestNotification;

public class SendTestNotificationCommandHandler : IRequestHandler<SendTestNotificationCommand, bool>
{
    private readonly IEnumerable<INotificationProvider> _providers;
    private readonly INotificationLogRepository _logRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IUnitOfWork _unitOfWork;

    public SendTestNotificationCommandHandler(
        IEnumerable<INotificationProvider> providers,
        INotificationLogRepository logRepository,
        ITenantContext tenantContext,
        IUnitOfWork unitOfWork)
    {
        _providers = providers;
        _logRepository = logRepository;
        _tenantContext = tenantContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(SendTestNotificationCommand request, CancellationToken cancellationToken)
    {
        var provider = _providers.FirstOrDefault(p => p.Channel == request.Channel);
        if (provider == null) return false;

        var tenantId = _tenantContext.TenantId;
        var log = new NotificationLog(
            tenantId,
            null,
            request.Channel,
            request.RecipientTarget,
            request.Subject,
            request.Body);

        await _logRepository.AddAsync(log, cancellationToken);

        try
        {
            bool success = await provider.SendAsync(request.RecipientTarget, request.Subject, request.Body, cancellationToken);
            if (success)
            {
                log.MarkAsSent();
            }
            else
            {
                log.MarkAsFailed("Test notification provider returned failure.");
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return success;
        }
        catch (Exception ex)
        {
            log.MarkAsFailed(ex.Message);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return false;
        }
    }
}