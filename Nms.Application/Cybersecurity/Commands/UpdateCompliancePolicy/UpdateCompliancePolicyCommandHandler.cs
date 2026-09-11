using MediatR;
using Nms.Application.Cybersecurity.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Cybersecurity.Commands.UpdateCompliancePolicy;

public class UpdateCompliancePolicyCommandHandler : IRequestHandler<UpdateCompliancePolicyCommand, CompliancePolicyDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCompliancePolicyCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CompliancePolicyDto?> Handle(UpdateCompliancePolicyCommand request, CancellationToken cancellationToken)
    {
        var policy = await _unitOfWork.CompliancePolicies.GetByIdAsync(request.Id, cancellationToken);
        if (policy == null)
            return null;

        policy.Update(
            request.Name,
            request.Description,
            request.Category,
            request.CheckType,
            request.Severity,
            request.IsActive,
            request.TargetVendor,
            request.TargetDeviceType,
            request.RuleConfigurationJson);

        _unitOfWork.CompliancePolicies.Update(policy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CompliancePolicyDto
        {
            Id = policy.Id,
            TenantId = policy.TenantId,
            Name = policy.Name,
            Description = policy.Description,
            Category = policy.Category,
            CheckType = policy.CheckType,
            Severity = policy.Severity,
            IsActive = policy.IsActive,
            TargetVendor = policy.TargetVendor,
            TargetDeviceType = policy.TargetDeviceType,
            RuleConfigurationJson = policy.RuleConfigurationJson,
            CreatedAtUtc = policy.CreatedAtUtc,
            CreatedBy = policy.CreatedBy,
            LastModifiedAtUtc = policy.LastModifiedAtUtc,
            LastModifiedBy = policy.LastModifiedBy
        };
    }
}