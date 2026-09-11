using MediatR;
using Nms.Application.ConfigurationBackups.Dtos;

namespace Nms.Application.ConfigurationBackups.Commands.CreateBackupSchedule;

public record CreateBackupScheduleCommand(
    Guid TenantId,
    string Name,
    int IntervalMinutes,
    Guid? DeviceId = null,
    string? Description = null) : IRequest<BackupScheduleDto>;