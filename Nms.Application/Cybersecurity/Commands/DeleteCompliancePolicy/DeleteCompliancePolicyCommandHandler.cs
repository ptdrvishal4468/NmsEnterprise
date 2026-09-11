using MediatR;
using Nms.Domain.Interfaces;

namespace Nms.Application.Cybersecurity.Commands.DeleteCompliancePolicy;

public class DeleteCompliancePolicyCommandHandler : IRequestHandler<DeleteCompliancePolicyCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCompliancePolicyCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteCompliancePolicyCommand request, CancellationToken cancellationToken)
    {
        var policy = await _unitOfWork.CompliancePolicies.GetByIdAsync(request.Id, cancellationToken);
        if (policy == null)
            return false;

        _unitOfWork.CompliancePolicies.Remove(policy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}