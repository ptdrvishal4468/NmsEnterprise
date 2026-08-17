using MediatR;
using Nms.Application.Customers.Dtos;

namespace Nms.Application.Customers.Commands.UpdateCustomerContact;

public record UpdateCustomerContactCommand(Guid CustomerId, Guid ContactId, CustomerContactInputDto Dto) : IRequest<CustomerContactDto>;