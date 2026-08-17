using FluentValidation;
using Nms.Domain.Interfaces;

namespace Nms.Application.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator(IUnitOfWork unitOfWork)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Customer ID is required.");

        RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage("Customer name is required.")
            .MaximumLength(200).WithMessage("Customer name must not exceed 200 characters.");

        RuleFor(x => x.Dto.Code)
            .NotEmpty().WithMessage("Customer code is required.")
            .MaximumLength(50).WithMessage("Customer code must not exceed 50 characters.")
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Customer code can only contain alphanumeric characters, underscores, and hyphens.")
            .MustAsync(async (cmd, code, ct) => !await unitOfWork.Customers.CodeExistsAsync(code.Trim().ToUpperInvariant(), cmd.Id, ct))
            .WithMessage("A customer with this code already exists in the tenant.");

        RuleFor(x => x.Dto.AccountNumber)
            .MaximumLength(100).WithMessage("Account number must not exceed 100 characters.");

        RuleFor(x => x.Dto.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.Dto.Status)
            .IsInEnum().WithMessage("Invalid customer status.");

        RuleFor(x => x.Dto.Tier)
            .IsInEnum().WithMessage("Invalid customer tier.");

        RuleFor(x => x.Dto.ParentCustomerId)
            .Must((cmd, parentId) => !parentId.HasValue || parentId.Value != cmd.Id)
            .WithMessage("A customer cannot be assigned as its own parent.");
    }
}