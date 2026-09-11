using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Vulnerabilities.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Vulnerabilities.Queries.GetSecurityAdvisoryById;

public class GetSecurityAdvisoryByIdQueryHandler : IRequestHandler<GetSecurityAdvisoryByIdQuery, SecurityAdvisoryDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetSecurityAdvisoryByIdQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<SecurityAdvisoryDto> Handle(GetSecurityAdvisoryByIdQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var sa = await _unitOfWork.SecurityAdvisories.GetByIdAsync(request.Id, cancellationToken);
        if (sa == null || sa.TenantId != tenantId)
        {
            throw new KeyNotFoundException($"Security Advisory with ID '{request.Id}' was not found.");
        }

        return new SecurityAdvisoryDto(
            sa.Id,
            sa.TenantId,
            sa.AdvisoryId,
            sa.Vendor,
            sa.Title,
            sa.Summary,
            sa.Severity,
            sa.RemediationGuidance,
            sa.ReferenceUrl,
            sa.PublishedAtUtc,
            sa.CreatedAtUtc);
    }
}