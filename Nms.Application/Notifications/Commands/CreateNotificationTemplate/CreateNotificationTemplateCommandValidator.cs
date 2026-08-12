using FluentValidation;

namespace Nms.Application.Notifications.Commands.CreateNotificationTemplate;

public class CreateNotificationTemplateCommandValidator : AbstractValidator<CreateNotificationTemplateCommand>
{
    public CreateNotificationTemplateCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Channel).IsInEnum();
        RuleFor(x => x.SubjectTemplate).NotEmpty().MaximumLength(500);
        RuleFor(x => x.BodyTemplate).NotEmpty();
        RuleFor(x => x.RecipientTarget).NotEmpty().MaximumLength(500);
    }
}