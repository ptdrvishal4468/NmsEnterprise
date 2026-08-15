using MediatR;
using Nms.Application.Auditing.Dtos;

namespace Nms.Application.Auditing.Queries.GetConfigurationChanges;

public record GetConfigurationChangesQuery(
    DateTime? FromUtc = null,
    DateTime? ToUtc = null,
    string? EntityName = null,
    int MaxCount = 50
) : IRequest<IReadOnlyList<ConfigurationChangeDto>>;