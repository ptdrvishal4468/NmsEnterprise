using MediatR;
using Nms.Domain.Interfaces;

namespace Nms.Application.Customers.Commands.DeleteCustomerContact;

public class DeleteCustomerContactCommandHandler : IRequestHandler<DeleteCustomerContactCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerContactCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteCustomerContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await _unitOfWork.CustomerContacts.GetByIdAsync(request.ContactId, cancellationToken);
        if (contact == null || contact.CustomerId != request.CustomerId)
        {
            return false;
        }

        _unitOfWork.CustomerContacts.Remove(contact);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}