using MediatR;
using Nms.Application.Customers.Dtos;

namespace Nms.Application.Customers.Queries.GetCustomerById;

public record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDto?>;