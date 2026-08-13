using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.ConfigurationBackups.Dtos;

namespace Nms.Application.ConfigurationBackups.Queries.GetBackupSchedulesPaged;

public record GetBackupSchedulesPagedQuery(
    Guid TenantId,
    Guid? DeviceId = null,
    bool? IsEnabled = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<PagedResult<BackupScheduleDto>>;