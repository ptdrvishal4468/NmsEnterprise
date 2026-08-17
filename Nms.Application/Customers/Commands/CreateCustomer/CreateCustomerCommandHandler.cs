using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Customers.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public CreateCustomerCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var dto = request.Dto;

        if (dto.ParentCustomerId.HasValue)
        {
            var parent = await _unitOfWork.Customers.GetByIdAsync(dto.ParentCustomerId.Value, cancellationToken);
            if (parent == null)
            {
                throw new KeyNotFoundException($"Parent customer with ID '{dto.ParentCustomerId.Value}' was not found.");
            }
        }

        var customer = new Customer(
            id: Guid.NewGuid(),
            tenantId: tenantId,
            name: dto.Name.Trim(),
            code: dto.Code.Trim().ToUpperInvariant(),
            accountNumber: dto.AccountNumber?.Trim(),
            description: dto.Description?.Trim(),
            status: dto.Status,
            tier: dto.Tier,
            parentCustomerId: dto.ParentCustomerId,
            metadataJson: dto.MetadataJson);

        await _unitOfWork.Customers.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        string? parentName = null;
        if (customer.ParentCustomerId.HasValue)
        {
            var parent = await _unitOfWork.Customers.GetByIdAsync(customer.ParentCustomerId.Value, cancellationToken);
            parentName = parent?.Name;
        }

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
            ContactCount = 0,
            ChildCustomerCount = 0,
            CreatedAtUtc = customer.CreatedAtUtc,
            CreatedBy = customer.CreatedBy,
            LastModifiedAtUtc = customer.LastModifiedAtUtc,
            LastModifiedBy = customer.LastModifiedBy
        };
    }
}