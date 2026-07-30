using Nms.Application.Common.Models;
using Nms.Application.Users.Dtos;

namespace Nms.Application.Users.Queries.GetUsersPaged;

public class GetUsersPagedQueryHandler
{
    public Task<PagedResult<UserDto>> HandleAsync(GetUsersPagedQuery query, CancellationToken cancellationToken = default)
    {
        var emptyList = new List<UserDto>();
        return Task.FromResult(new PagedResult<UserDto>(emptyList, 0, query.PageIndex, query.PageSize));
    }
}