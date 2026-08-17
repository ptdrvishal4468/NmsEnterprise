using MediatR;
using Nms.Application.Customers.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Customers.Queries.GetCustomerHierarchy;

public class GetCustomerHierarchyQueryHandler : IRequestHandler<GetCustomerHierarchyQuery, CustomerHierarchyDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCustomerHierarchyQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerHierarchyDto?> Handle(GetCustomerHierarchyQuery request, CancellationToken cancellationToken)
    {
        var rootCustomer = await _unitOfWork.Customers.GetByIdAsync(request.Id, cancellationToken);
        if (rootCustomer == null)
        {
            return null;
        }

        var visited = new HashSet<Guid> { rootCustomer.Id };
        return await BuildHierarchyRecursiveAsync(rootCustomer.Id, visited, cancellationToken);
    }

    private async Task<CustomerHierarchyDto?> BuildHierarchyRecursiveAsync(
        Guid customerId,
        HashSet<Guid> visited,
        CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(customerId, cancellationToken);
        if (customer == null)
        {
            return null;
        }

        var children = await _unitOfWork.Customers.GetChildrenAsync(customerId, cancellationToken);
        var childDtos = new List<CustomerHierarchyDto>();

        foreach (var child in children)
        {
            if (visited.Add(child.Id))
            {
                var childHierarchy = await BuildHierarchyRecursiveAsync(child.Id, visited, cancellationToken);
                if (childHierarchy != null)
                {
                    childDtos.Add(childHierarchy);
                }
            }
        }

        return new CustomerHierarchyDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Code = customer.Code,
            Status = customer.Status,
            Tier = customer.Tier,
            ParentCustomerId = customer.ParentCustomerId,
            Children = childDtos
        };
    }
}