using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nms.Infrastructure.Data;

namespace Nms.Infrastructure.Security;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // 1. Ensure user is authenticated
        if (context.User?.Identity == null || !context.User.Identity.IsAuthenticated)
        {
            return;
        }

        // 2. Extract UserId claim
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? context.User.FindFirst("sub")?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return;
        }

        // 3. Resolve DbContext within scope to check dynamic role permissions
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NmsDbContext>();

        var hasPermission = await (
            from ur in dbContext.UserRoles
            join rp in dbContext.RolePermissions on ur.RoleId equals rp.RoleId
            join p in dbContext.Permissions on rp.PermissionId equals p.Id
            where ur.UserId == userId && p.PermissionKey == requirement.Permission
            select p
        ).AnyAsync();

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}