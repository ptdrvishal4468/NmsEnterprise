using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Application.ThreatDetection.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.ThreatDetection.Queries.GetThreatIndicatorsPaged;

public class GetThreatIndicatorsPagedQueryHandler : IRequestHandler<GetThreatIndicatorsPagedQuery, PagedResult<ThreatIndicatorDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetThreatIndicatorsPagedQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<PagedResult<ThreatIndicatorDto>> Handle(GetThreatIndicatorsPagedQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.ThreatIndicators.GetPagedAsync(
            _tenantContext.TenantId,
            request.PageNumber,
            request.PageSize,
            request.ThreatType,
            request.Severity,
            request.Status,
            request.DeviceId,
            cancellationToken);

        var dtos = items.Select(i => new ThreatIndicatorDto
        {
            Id = i.Id,
            TenantId = i.TenantId,
            ThreatType = i.ThreatType,
            Severity = i.Severity,
            Status = i.Status,
            Title = i.Title,
            Description = i.Description,
            SourceIp = i.SourceIp,
            TargetDeviceId = i.TargetDeviceId,
            TargetDeviceName = i.TargetDevice?.Name,
            TargetUser = i.TargetUser,
            AttemptCount = i.AttemptCount,
            FirstDetectedAtUtc = i.FirstDetectedAtUtc,
            LastDetectedAtUtc = i.LastDetectedAtUtc,
            ResolutionNotes = i.ResolutionNotes,
            IndicatorMetadataJson = i.IndicatorMetadataJson
        }).ToList();

        return new PagedResult<ThreatIndicatorDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}