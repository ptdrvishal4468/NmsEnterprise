using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Vulnerabilities.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Vulnerabilities.Commands.CreateSecurityAdvisory;

public class CreateSecurityAdvisoryCommandHandler : IRequestHandler<CreateSecurityAdvisoryCommand, SecurityAdvisoryDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public CreateSecurityAdvisoryCommandHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<SecurityAdvisoryDto> Handle(CreateSecurityAdvisoryCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var existing = await _unitOfWork.SecurityAdvisories.GetByAdvisoryIdAsync(tenantId, request.AdvisoryId, cancellationToken);
        if (existing != null)
        {
            throw new InvalidOperationException($"Security Advisory with ID '{request.AdvisoryId}' already exists.");
        }

        var advisory = new SecurityAdvisory(
            Guid.NewGuid(),
            tenantId,
            request.AdvisoryId,
            request.Vendor,
            request.Title,
            request.Summary,
            request.Severity,
            request.RemediationGuidance,
            request.ReferenceUrl,
            request.PublishedAtUtc);

        await _unitOfWork.SecurityAdvisories.AddAsync(advisory, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SecurityAdvisoryDto(
            advisory.Id,
            advisory.TenantId,
            advisory.AdvisoryId,
            advisory.Vendor,
            advisory.Title,
            advisory.Summary,
            advisory.Severity,
            advisory.RemediationGuidance,
            advisory.ReferenceUrl,
            advisory.PublishedAtUtc,
            advisory.CreatedAtUtc);
    }
}