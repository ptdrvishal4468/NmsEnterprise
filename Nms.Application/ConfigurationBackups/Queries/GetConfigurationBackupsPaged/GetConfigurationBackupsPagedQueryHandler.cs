using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.ConfigurationBackups.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.ConfigurationBackups.Queries.GetConfigurationBackupsPaged;

public class GetConfigurationBackupsPagedQueryHandler : IRequestHandler<GetConfigurationBackupsPagedQuery, PagedResult<ConfigurationBackupDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetConfigurationBackupsPagedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<ConfigurationBackupDto>> Handle(GetConfigurationBackupsPagedQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.ConfigurationBackups.GetBackupsPagedAsync(
            request.TenantId,
            request.DeviceId,
            request.Status,
            request.TriggerType,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(b => new ConfigurationBackupDto
        {
            Id = b.Id,
            TenantId = b.TenantId,
            DeviceId = b.DeviceId,
            DeviceName = b.Device?.Name ?? string.Empty,
            DeviceIpAddress = b.Device?.IpAddress ?? string.Empty,
            VersionNumber = b.VersionNumber,
            ChecksumSha256 = b.ChecksumSha256,
            FileSizeBytes = b.FileSizeBytes,
            Status = b.Status,
            TriggerType = b.TriggerType,
            FailureReason = b.FailureReason,
            TimestampUtc = b.TimestampUtc,
            IsEligibleForRestore = b.IsEligibleForRestore,
            RestorePreparationNotes = b.RestorePreparationNotes
        }).ToList();

        return new PagedResult<ConfigurationBackupDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}