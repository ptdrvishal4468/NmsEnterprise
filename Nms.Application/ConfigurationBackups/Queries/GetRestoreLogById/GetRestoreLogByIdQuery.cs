using MediatR;
using Nms.Application.ConfigurationBackups.Dtos;

namespace Nms.Application.ConfigurationBackups.Queries.GetRestoreLogById;

public record GetRestoreLogByIdQuery(Guid RestoreLogId) : IRequest<ConfigurationRestoreLogDto?>;