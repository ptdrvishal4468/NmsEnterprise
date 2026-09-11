using MediatR;
using Nms.Application.Customers.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Customers.Commands.UpdateCustomerContact;

public class UpdateCustomerContactCommandHandler : IRequestHandler<UpdateCustomerContactCommand, CustomerContactDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerContactCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerContactDto> Handle(UpdateCustomerContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await _unitOfWork.CustomerContacts.GetByIdAsync(request.ContactId, cancellationToken);
        if (contact == null || contact.CustomerId != request.CustomerId)
        {
            throw new KeyNotFoundException($"Contact with ID '{request.ContactId}' was not found for customer '{request.CustomerId}'.");
        }

        var dto = request.Dto;

        if (dto.IsPrimary && !contact.IsPrimary)
        {
            var currentPrimary = await _unitOfWork.CustomerContacts.GetPrimaryContactAsync(request.CustomerId, cancellationToken);
            if (currentPrimary != null && currentPrimary.Id != contact.Id)
            {
                currentPrimary.SetPrimary(false);
                _unitOfWork.CustomerContacts.Update(currentPrimary);
            }
        }

        contact.UpdateContactDetails(
            firstName: dto.FirstName.Trim(),
            lastName: dto.LastName.Trim(),
            email: dto.Email.Trim().ToLowerInvariant(),
            phoneNumber: dto.PhoneNumber?.Trim(),
            jobTitle: dto.JobTitle?.Trim(),
            contactType: dto.ContactType,
            isPrimary: dto.IsPrimary);

        _unitOfWork.CustomerContacts.Update(contact);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CustomerContactDto
        {
            Id = contact.Id,
            TenantId = contact.TenantId,
            CustomerId = contact.CustomerId,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            Email = contact.Email,
            PhoneNumber = contact.PhoneNumber,
            JobTitle = contact.JobTitle,
            ContactType = contact.ContactType,
            IsPrimary = contact.IsPrimary,
            CreatedAtUtc = contact.CreatedAtUtc,
            LastModifiedAtUtc = contact.LastModifiedAtUtc
        };
    }
}