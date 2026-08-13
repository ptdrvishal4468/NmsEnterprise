using MediatR;
using Nms.Application.ConfigurationBackups.Dtos;

namespace Nms.Application.ConfigurationBackups.Commands.PrepareRestore;

public record PrepareRestoreCommand(
    Guid TenantId,
    Guid BackupId,
    string? Notes = null) : IRequest<RestorePreparationDto>;