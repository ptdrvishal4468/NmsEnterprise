using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Notifications.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Notifications.Commands.CreateNotificationTemplate;

public class CreateNotificationTemplateCommandHandler : IRequestHandler<CreateNotificationTemplateCommand, NotificationTemplateDto>
{
    private readonly INotificationTemplateRepository _templateRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IUnitOfWork _unitOfWork;

    public CreateNotificationTemplateCommandHandler(
        INotificationTemplateRepository templateRepository,
        ITenantContext tenantContext,
        IUnitOfWork unitOfWork)
    {
        _templateRepository = templateRepository;
        _tenantContext = tenantContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<NotificationTemplateDto> Handle(CreateNotificationTemplateCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;

        var template = new NotificationTemplate(
            tenantId,
            request.Name,
            request.Channel,
            request.SubjectTemplate,
            request.BodyTemplate,
            request.RecipientTarget);

        await _templateRepository.AddAsync(template, cancellationToken);
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