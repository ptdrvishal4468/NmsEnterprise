using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Customers.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Customers.Commands.AddCustomerContact;

public class AddCustomerContactCommandHandler : IRequestHandler<AddCustomerContactCommand, CustomerContactDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public AddCustomerContactCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<CustomerContactDto> Handle(AddCustomerContactCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID '{request.CustomerId}' was not found.");
        }

        var dto = request.Dto;

        if (dto.IsPrimary)
        {
            var currentPrimary = await _unitOfWork.CustomerContacts.GetPrimaryContactAsync(request.CustomerId, cancellationToken);
            if (currentPrimary != null)
            {
                currentPrimary.SetPrimary(false);
                _unitOfWork.CustomerContacts.Update(currentPrimary);
            }
        }

        var contact = new CustomerContact(
            id: Guid.NewGuid(),
            tenantId: _tenantContext.TenantId,
            customerId: request.CustomerId,
            firstName: dto.FirstName.Trim(),
            lastName: dto.LastName.Trim(),
            email: dto.Email.Trim().ToLowerInvariant(),
            phoneNumber: dto.PhoneNumber?.Trim(),
            jobTitle: dto.JobTitle?.Trim(),
            contactType: dto.ContactType,
            isPrimary: dto.IsPrimary);

        await _unitOfWork.CustomerContacts.AddAsync(contact, cancellationToken);
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