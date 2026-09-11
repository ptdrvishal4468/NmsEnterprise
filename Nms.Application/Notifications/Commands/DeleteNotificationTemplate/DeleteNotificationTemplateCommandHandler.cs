using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Interfaces;

namespace Nms.Application.Notifications.Commands.DeleteNotificationTemplate;

public class DeleteNotificationTemplateCommandHandler : IRequestHandler<DeleteNotificationTemplateCommand>
{
    private readonly INotificationTemplateRepository _templateRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteNotificationTemplateCommandHandler(
        INotificationTemplateRepository templateRepository,
        ITenantContext tenantContext,
        IUnitOfWork unitOfWork)
    {
        _templateRepository = templateRepository;
        _tenantContext = tenantContext;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteNotificationTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await _templateRepository.GetByIdAsync(request.Id, cancellationToken);
        if (template == null || template.TenantId != _tenantContext.TenantId)
        {
            throw new KeyNotFoundException($"Notification template with ID '{request.Id}' was not found.");
        }

        _templateRepository.Remove(template);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}