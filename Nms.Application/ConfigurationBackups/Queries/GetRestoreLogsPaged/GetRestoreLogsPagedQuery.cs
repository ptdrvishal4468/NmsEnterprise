using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.ConfigurationBackups.Dtos;

namespace Nms.Application.ConfigurationBackups.Queries.GetRestoreLogsPaged;

public record GetRestoreLogsPagedQuery(
    Guid? DeviceId = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<PagedResult<ConfigurationRestoreLogDto>>;