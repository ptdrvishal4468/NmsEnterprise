using MediatR;
using Nms.Application.Customers.Dtos;

namespace Nms.Application.Customers.Commands.UpdateCustomer;

public record UpdateCustomerCommand(Guid Id, UpdateCustomerDto Dto) : IRequest<CustomerDto>;