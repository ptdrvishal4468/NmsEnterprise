using MediatR;
using Nms.Application.ConfigurationBackups.Dtos;

namespace Nms.Application.ConfigurationBackups.Queries.DownloadConfigurationBackup;

public record DownloadConfigurationBackupQuery(
    Guid TenantId,
    Guid BackupId) : IRequest<ConfigurationDownloadDto>;