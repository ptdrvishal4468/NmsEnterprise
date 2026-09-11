using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Cybersecurity.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Cybersecurity.Queries.GetCompliancePoliciesPaged;

public class GetCompliancePoliciesPagedQueryHandler : IRequestHandler<GetCompliancePoliciesPagedQuery, PagedResult<CompliancePolicyDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCompliancePoliciesPagedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<CompliancePolicyDto>> Handle(GetCompliancePoliciesPagedQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.CompliancePolicies.GetPagedAsync(
            request.PageIndex,
            request.PageSize,
            request.SearchTerm,
            request.Category,
            request.Severity,
            request.IsActive,
            cancellationToken);

        var dtos = items.Select(p => new CompliancePolicyDto
        {
            Id = p.Id,
            TenantId = p.TenantId,
            Name = p.Name,
            Description = p.Description,
            Category = p.Category,
            CheckType = p.CheckType,
            Severity = p.Severity,
            IsActive = p.IsActive,
            TargetVendor = p.TargetVendor,
            TargetDeviceType = p.TargetDeviceType,
            RuleConfigurationJson = p.RuleConfigurationJson,
            CreatedAtUtc = p.CreatedAtUtc,
            CreatedBy = p.CreatedBy,
            LastModifiedAtUtc = p.LastModifiedAtUtc,
            LastModifiedBy = p.LastModifiedBy
        }).ToList();

        return new PagedResult<CompliancePolicyDto>(dtos, totalCount, request.PageIndex, request.PageSize);
    }
}