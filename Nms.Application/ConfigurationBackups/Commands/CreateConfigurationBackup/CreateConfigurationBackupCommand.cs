using MediatR;
using Nms.Application.ConfigurationBackups.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.ConfigurationBackups.Commands.CreateConfigurationBackup;

public record CreateConfigurationBackupCommand(
    Guid TenantId,
    Guid DeviceId,
    BackupTriggerType TriggerType = BackupTriggerType.Manual) : IRequest<ConfigurationBackupDto>;