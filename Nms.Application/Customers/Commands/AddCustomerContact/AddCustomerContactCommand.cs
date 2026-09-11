using MediatR;
using Nms.Application.Customers.Dtos;

namespace Nms.Application.Customers.Commands.AddCustomerContact;

public record AddCustomerContactCommand(Guid CustomerId, CustomerContactInputDto Dto) : IRequest<CustomerContactDto>;