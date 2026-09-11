using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Customers.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Customers.Queries.GetCustomersPaged;

public class GetCustomersPagedQueryHandler : IRequestHandler<GetCustomersPagedQuery, PagedResult<CustomerDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCustomersPagedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<CustomerDto>> Handle(GetCustomersPagedQuery request, CancellationToken cancellationToken)
    {
        var pageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
        var pageSize = request.PageSize < 1 ? 20 : (request.PageSize > 100 ? 100 : request.PageSize);

        var allCustomers = await _unitOfWork.Customers.GetAllAsync(cancellationToken);

        var query = allCustomers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToUpperInvariant();
            query = query.Where(c =>
                c.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                c.Code.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (c.AccountNumber != null && c.AccountNumber.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(c => c.Status == request.Status.Value);
        }

        if (request.Tier.HasValue)
        {
            query = query.Where(c => c.Tier == request.Tier.Value);
        }

        if (request.ParentCustomerId.HasValue)
        {
            query = query.Where(c => c.ParentCustomerId == request.ParentCustomerId.Value);
        }

        var totalCount = query.Count();
        var items = query
            .OrderBy(c => c.Name)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var customerLookup = allCustomers.ToDictionary(c => c.Id, c => c.Name);
        var dtos = new List<CustomerDto>();

        foreach (var customer in items)
        {
            string? parentName = null;
            if (customer.ParentCustomerId.HasValue && customerLookup.TryGetValue(customer.ParentCustomerId.Value, out var pName))
            {
                parentName = pName;
            }

            var contacts = await _unitOfWork.CustomerContacts.GetByCustomerIdAsync(customer.Id, cancellationToken);
            var children = await _unitOfWork.Customers.GetChildrenAsync(customer.Id, cancellationToken);

            dtos.Add(new CustomerDto
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
            });
        }

        return new PagedResult<CustomerDto>(dtos, totalCount, pageIndex, pageSize);
    }
}