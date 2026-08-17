using MediatR;
using Nms.Application.Customers.Dtos;

namespace Nms.Application.Customers.Queries.GetCustomerContacts;

public record GetCustomerContactsQuery(Guid CustomerId) : IRequest<IReadOnlyList<CustomerContactDto>>;