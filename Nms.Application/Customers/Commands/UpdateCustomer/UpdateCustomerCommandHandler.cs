using MediatR;
using Nms.Application.Customers.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, CustomerDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerDto> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(request.Id, cancellationToken);
        if (customer == null)
        {
            throw new KeyNotFoundException($"Customer with ID '{request.Id}' was not found.");
        }

        var dto = request.Dto;

        if (dto.ParentCustomerId.HasValue)
        {
            if (dto.ParentCustomerId.Value == customer.Id)
            {
                throw new InvalidOperationException("A customer cannot be assigned as its own parent.");
            }

            var parent = await _unitOfWork.Customers.GetByIdAsync(dto.ParentCustomerId.Value, cancellationToken);
            if (parent == null)
            {
                throw new KeyNotFoundException($"Parent customer with ID '{dto.ParentCustomerId.Value}' was not found.");
            }

            var isCircular = await _unitOfWork.Customers.HasCircularHierarchyAsync(customer.Id, dto.ParentCustomerId.Value, cancellationToken);
            if (isCircular)
            {
                throw new InvalidOperationException("Circular customer hierarchy relationship detected.");
            }
        }

        customer.UpdateDetails(
            name: dto.Name.Trim(),
            code: dto.Code.Trim().ToUpperInvariant(),
            accountNumber: dto.AccountNumber?.Trim(),
            description: dto.Description?.Trim(),
            status: dto.Status,
            tier: dto.Tier,
            parentCustomerId: dto.ParentCustomerId,
            metadataJson: dto.MetadataJson);

        _unitOfWork.Customers.Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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