using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Cybersecurity.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Cybersecurity.Commands.CreateCompliancePolicy;

public class CreateCompliancePolicyCommandHandler : IRequestHandler<CreateCompliancePolicyCommand, CompliancePolicyDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public CreateCompliancePolicyCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<CompliancePolicyDto> Handle(CreateCompliancePolicyCommand request, CancellationToken cancellationToken)
    {
        var policy = new CompliancePolicy(
            id: Guid.NewGuid(),
            tenantId: _tenantContext.TenantId,
            name: request.Name,
            description: request.Description,
            category: request.Category,
            checkType: request.CheckType,
            severity: request.Severity,
            isActive: request.IsActive,
            targetVendor: request.TargetVendor,
            targetDeviceType: request.TargetDeviceType,
            ruleConfigurationJson: request.RuleConfigurationJson);

        await _unitOfWork.CompliancePolicies.AddAsync(policy, cancellationToken);
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