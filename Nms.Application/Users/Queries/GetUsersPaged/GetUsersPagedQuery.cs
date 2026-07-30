using Nms.Application.Common.Models;
using Nms.Application.Users.Dtos;

namespace Nms.Application.Users.Queries.GetUsersPaged;

public record GetUsersPagedQuery(
    Guid TenantId,
    int PageIndex = 1,
    int PageSize = 10,
    string? SearchTerm = null
);