using FluentValidation;

namespace Nms.Application.Notifications.Commands.SendTestNotification;

public class SendTestNotificationCommandValidator : AbstractValidator<SendTestNotificationCommand>
{
    public SendTestNotificationCommandValidator()
    {
        RuleFor(x => x.Channel).IsInEnum();
        RuleFor(x => x.RecipientTarget).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Body).NotEmpty();
    }
}