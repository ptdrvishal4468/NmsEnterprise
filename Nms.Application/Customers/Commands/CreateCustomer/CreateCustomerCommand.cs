using MediatR;
using Nms.Application.Customers.Dtos;

namespace Nms.Application.Customers.Commands.CreateCustomer;

public record CreateCustomerCommand(CreateCustomerDto Dto) : IRequest<CustomerDto>;