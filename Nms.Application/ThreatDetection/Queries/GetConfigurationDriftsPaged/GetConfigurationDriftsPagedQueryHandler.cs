using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Application.ThreatDetection.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.ThreatDetection.Queries.GetConfigurationDriftsPaged;

public class GetConfigurationDriftsPagedQueryHandler : IRequestHandler<GetConfigurationDriftsPagedQuery, PagedResult<ConfigurationDriftRecordDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetConfigurationDriftsPagedQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<PagedResult<ConfigurationDriftRecordDto>> Handle(GetConfigurationDriftsPagedQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.ConfigurationDrifts.GetPagedAsync(
            _tenantContext.TenantId,
            request.PageNumber,
            request.PageSize,
            request.DeviceId,
            request.HasDrift,
            cancellationToken);

        var dtos = items.Select(r => new ConfigurationDriftRecordDto
        {
            Id = r.Id,
            TenantId = r.TenantId,
            DeviceId = r.DeviceId,
            DeviceName = r.Device?.Name ?? string.Empty,
            BaselineBackupId = r.BaselineBackupId,
            CurrentBackupId = r.CurrentBackupId,
            HasDrift = r.HasDrift,
            AddedLinesCount = r.AddedLinesCount,
            RemovedLinesCount = r.RemovedLinesCount,
            ModifiedLinesCount = r.ModifiedLinesCount,
            DifferencesJson = r.DifferencesJson,
            DetectedAtUtc = r.DetectedAtUtc,
            Severity = r.Severity,
            IsAcknowledged = r.IsAcknowledged,
            AcknowledgmentNotes = r.AcknowledgmentNotes
        }).ToList();

        return new PagedResult<ConfigurationDriftRecordDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}