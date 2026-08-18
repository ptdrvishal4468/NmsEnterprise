using MediatR;
using Nms.Application.Cybersecurity.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Cybersecurity.Queries.GetCompliancePolicyById;

public class GetCompliancePolicyByIdQueryHandler : IRequestHandler<GetCompliancePolicyByIdQuery, CompliancePolicyDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCompliancePolicyByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CompliancePolicyDto?> Handle(GetCompliancePolicyByIdQuery request, CancellationToken cancellationToken)
    {
        var policy = await _unitOfWork.CompliancePolicies.GetByIdAsync(request.Id, cancellationToken);
        if (policy == null)
            return null;

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