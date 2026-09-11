using FluentValidation;
using Nms.Domain.Interfaces;

namespace Nms.Application.Customers.Commands.AddCustomerContact;

public class AddCustomerContactCommandValidator : AbstractValidator<AddCustomerContactCommand>
{
    public AddCustomerContactCommandValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required.");

        RuleFor(x => x.Dto.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.Dto.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.Dto.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(200).WithMessage("Email must not exceed 200 characters.")
            .MustAsync(async (cmd, email, ct) => !await unitOfWork.CustomerContacts.EmailExistsInCustomerAsync(cmd.CustomerId, email.Trim().ToLowerInvariant(), null, ct))
            .WithMessage("A contact with this email address already exists for this customer.");

        RuleFor(x => x.Dto.PhoneNumber)
            .MaximumLength(50).WithMessage("Phone number must not exceed 50 characters.");

        RuleFor(x => x.Dto.JobTitle)
            .MaximumLength(100).WithMessage("Job title must not exceed 100 characters.");

        RuleFor(x => x.Dto.ContactType)
            .IsInEnum().WithMessage("Invalid contact type.");
    }
}