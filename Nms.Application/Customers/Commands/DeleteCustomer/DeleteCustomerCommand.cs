using MediatR;

namespace Nms.Application.Customers.Commands.DeleteCustomer;

public record DeleteCustomerCommand(Guid Id) : IRequest<bool>;