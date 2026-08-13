using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.ConfigurationBackups.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.ConfigurationBackups.Queries.GetBackupSchedulesPaged;

public class GetBackupSchedulesPagedQueryHandler : IRequestHandler<GetBackupSchedulesPagedQuery, PagedResult<BackupScheduleDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBackupSchedulesPagedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<BackupScheduleDto>> Handle(GetBackupSchedulesPagedQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.BackupSchedules.GetSchedulesPagedAsync(
            request.TenantId,
            request.DeviceId,
            request.IsEnabled,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(s => new BackupScheduleDto
        {
            Id = s.Id,
            TenantId = s.TenantId,
            Name = s.Name,
            Description = s.Description,
            DeviceId = s.DeviceId,
            DeviceName = s.Device?.Name,
            IntervalMinutes = s.IntervalMinutes,
            IsEnabled = s.IsEnabled,
            LastRunUtc = s.LastRunUtc,
            NextRunUtc = s.NextRunUtc,
            CreatedAtUtc = s.CreatedAtUtc
        }).ToList();

        return new PagedResult<BackupScheduleDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}