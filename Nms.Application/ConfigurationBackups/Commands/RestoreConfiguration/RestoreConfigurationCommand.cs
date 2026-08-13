using MediatR;
using Nms.Application.ConfigurationBackups.Dtos;

namespace Nms.Application.ConfigurationBackups.Commands.RestoreConfiguration;

public record RestoreConfigurationCommand(Guid BackupId) : IRequest<RestoreExecutionResultDto>;