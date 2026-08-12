using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Notifications.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Notifications.Commands.UpdateNotificationTemplate;

public class UpdateNotificationTemplateCommandHandler : IRequestHandler<UpdateNotificationTemplateCommand, NotificationTemplateDto>
{
    private readonly INotificationTemplateRepository _templateRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateNotificationTemplateCommandHandler(
        INotificationTemplateRepository templateRepository,
        ITenantContext tenantContext,
        IUnitOfWork unitOfWork)
    {
        _templateRepository = templateRepository;
        _tenantContext = tenantContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<NotificationTemplateDto> Handle(UpdateNotificationTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await _templateRepository.GetByIdAsync(request.Id, cancellationToken);
        if (template == null || template.TenantId != _tenantContext.TenantId)
        {
            throw new KeyNotFoundException($"Notification template with ID '{request.Id}' was not found.");
        }

        template.Update(
            request.Name,
            request.Channel,
            request.SubjectTemplate,
            request.BodyTemplate,
            request.RecipientTarget);

        _templateRepository.Update(template);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new NotificationTemplateDto(
            template.Id,
            template.TenantId,
            template.Name,
            template.Channel,
            template.SubjectTemplate,
            template.BodyTemplate,
            template.RecipientTarget,
            template.IsEnabled,
            template.CreatedAtUtc,
            template.LastModifiedAtUtc);
    }
}