using MediatR;
using Nms.Application.Customers.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Customers.Queries.GetCustomerContacts;

public class GetCustomerContactsQueryHandler : IRequestHandler<GetCustomerContactsQuery, IReadOnlyList<CustomerContactDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCustomerContactsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<CustomerContactDto>> Handle(GetCustomerContactsQuery request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID '{request.CustomerId}' was not found.");
        }

        var contacts = await _unitOfWork.CustomerContacts.GetByCustomerIdAsync(request.CustomerId, cancellationToken);

        return contacts
            .OrderByDescending(c => c.IsPrimary)
            .ThenBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Select(c => new CustomerContactDto
            {
                Id = contact_select_id(c),
                TenantId = c.TenantId,
                CustomerId = c.CustomerId,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                JobTitle = c.JobTitle,
                ContactType = c.ContactType,
                IsPrimary = c.IsPrimary,
                CreatedAtUtc = c.CreatedAtUtc,
                LastModifiedAtUtc = c.LastModifiedAtUtc
            })
            .ToList();
    }

    private static Guid contact_select_id(Domain.Entities.CustomerContact c) => c.Id;
}