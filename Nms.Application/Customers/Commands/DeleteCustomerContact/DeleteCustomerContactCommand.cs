using MediatR;

namespace Nms.Application.Customers.Commands.DeleteCustomerContact;

public record DeleteCustomerContactCommand(Guid CustomerId, Guid ContactId) : IRequest<bool>;