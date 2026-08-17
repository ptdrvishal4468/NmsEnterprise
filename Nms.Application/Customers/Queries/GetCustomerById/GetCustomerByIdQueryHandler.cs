using MediatR;
using Nms.Application.Customers.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCustomerByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(request.Id, cancellationToken);
        if (customer == null)
        {
            return null;
        }

        string? parentName = null;
        if (customer.ParentCustomerId.HasValue)
        {
            var parent = await _unitOfWork.Customers.GetByIdAsync(customer.ParentCustomerId.Value, cancellationToken);
            parentName = parent?.Name;
        }

        var contacts = await _unitOfWork.CustomerContacts.GetByCustomerIdAsync(customer.Id, cancellationToken);
        var children = await _unitOfWork.Customers.GetChildrenAsync(customer.Id, cancellationToken);

        return new CustomerDto
        {
            Id = customer.Id,
            TenantId = customer.TenantId,
            Name = customer.Name,
            Code = customer.Code,
            AccountNumber = customer.AccountNumber,
            Description = customer.Description,
            Status = customer.Status,
            Tier = customer.Tier,
            ParentCustomerId = customer.ParentCustomerId,
            ParentCustomerName = parentName,
            MetadataJson = customer.MetadataJson,
            ContactCount = contacts.Count,
            ChildCustomerCount = children.Count,
            CreatedAtUtc = customer.CreatedAtUtc,
            CreatedBy = customer.CreatedBy,
            LastModifiedAtUtc = customer.LastModifiedAtUtc,
            LastModifiedBy = customer.LastModifiedBy
        };
    }
}