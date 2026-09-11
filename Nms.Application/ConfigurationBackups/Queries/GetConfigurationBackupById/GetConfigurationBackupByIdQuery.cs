using MediatR;
using Nms.Application.ConfigurationBackups.Dtos;

namespace Nms.Application.ConfigurationBackups.Queries.GetConfigurationBackupById;

public record GetConfigurationBackupByIdQuery(
    Guid TenantId,
    Guid BackupId) : IRequest<ConfigurationBackupDto>;