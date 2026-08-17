using MediatR;
using Nms.Domain.Interfaces;

namespace Nms.Application.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(request.Id, cancellationToken);
        if (customer == null)
        {
            return false;
        }

        var children = await _unitOfWork.Customers.GetChildrenAsync(request.Id, cancellationToken);
        if (children.Count > 0)
        {
            throw new InvalidOperationException("Cannot delete a customer that has active child sub-accounts. Reassign or remove child accounts first.");
        }

        _unitOfWork.Customers.Remove(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}