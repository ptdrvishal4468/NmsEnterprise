using Nms.Application.ConfigurationBackups.Dtos;

namespace Nms.Application.Common.Interfaces;

public interface IConfigurationRestoreEngine
{
    Task<RestoreExecutionResultDto> ExecuteRestoreAsync(
        Guid tenantId,
        Guid deviceId,
        Guid backupId,
        string initiatedBy,
        CancellationToken cancellationToken = default);
}