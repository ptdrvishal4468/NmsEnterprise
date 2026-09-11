using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.ConfigurationBackups.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.ConfigurationBackups.Queries.GetConfigurationBackupsPaged;

public record GetConfigurationBackupsPagedQuery(
    Guid TenantId,
    Guid? DeviceId = null,
    BackupStatus? Status = null,
    BackupTriggerType? TriggerType = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<PagedResult<ConfigurationBackupDto>>;