using FluentValidation;

namespace Nms.Application.PollProfiles.Commands.UpdatePollProfile;

public class UpdatePollProfileCommandValidator : AbstractValidator<UpdatePollProfileCommand>
{
    public UpdatePollProfileCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Profile ID is required.");

        RuleFor(v => v.Dto.Name)
            .NotEmpty().WithMessage("Profile name is required.")
            .MaximumLength(100).WithMessage("Profile name must not exceed 100 characters.");

        RuleFor(v => v.Dto.IntervalSeconds)
            .GreaterThanOrEqualTo(10).WithMessage("Interval must be at least 10 seconds.");

        RuleFor(v => v.Dto.TimeoutSeconds)
            .GreaterThan(0).WithMessage("Timeout must be greater than 0 seconds.")
            .LessThanOrEqualTo(60).WithMessage("Timeout must not exceed 60 seconds.");

        RuleFor(v => v.Dto.RetryCount)
            .GreaterThanOrEqualTo(0).WithMessage("Retry count cannot be negative.")
            .LessThanOrEqualTo(5).WithMessage("Retry count must not exceed 5.");
    }
}