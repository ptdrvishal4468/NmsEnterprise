using Nms.Application.Users.Dtos;

namespace Nms.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler
{
    public Task<UserDto?> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken = default)
    {
        // Query read-model for user details
        return Task.FromResult<UserDto?>(null);
    }
}