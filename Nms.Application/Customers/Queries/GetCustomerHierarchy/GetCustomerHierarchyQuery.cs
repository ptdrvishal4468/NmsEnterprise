using MediatR;
using Nms.Application.Customers.Dtos;

namespace Nms.Application.Customers.Queries.GetCustomerHierarchy;

public record GetCustomerHierarchyQuery(Guid Id) : IRequest<CustomerHierarchyDto?>;