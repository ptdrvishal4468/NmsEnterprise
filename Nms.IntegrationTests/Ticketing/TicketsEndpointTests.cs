using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Api.Controllers;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;
using Xunit;

namespace Nms.IntegrationTests.Ticketing;

public class TicketsEndpointTests
{
    [Fact]
    public void TicketsController_ShouldHaveAuthorizeAttributeAndCorrectRoute()
    {
        var type = typeof(TicketsController);

        var authorizeAttr = type.GetCustomAttribute<AuthorizeAttribute>();
        var routeAttr = type.GetCustomAttribute<RouteAttribute>();

        Assert.NotNull(authorizeAttr);
        Assert.NotNull(routeAttr);
        Assert.Equal("api/v1/tickets", routeAttr.Template);
    }

    [Theory]
    [InlineData(nameof(TicketsController.GetTicketsPaged), Permissions.Ticketing.View)]
    [InlineData(nameof(TicketsController.GetTicketById), Permissions.Ticketing.View)]
    [InlineData(nameof(TicketsController.CreateTicket), Permissions.Ticketing.Create)]
    [InlineData(nameof(TicketsController.UpdateTicket), Permissions.Ticketing.Update)]
    [InlineData(nameof(TicketsController.CloseTicket), Permissions.Ticketing.Update)]
    [InlineData(nameof(TicketsController.SyncTicket), Permissions.Ticketing.Sync)]
    [InlineData(nameof(TicketsController.GetTicketSyncLogs), Permissions.Ticketing.View)]
    public void TicketsController_Endpoints_ShouldEnforceDynamicRbacPermissions(string methodName, string expectedPermission)
    {
        var method = typeof(TicketsController).GetMethod(methodName);
        Assert.NotNull(method);

        var permissionAttr = method.GetCustomAttribute<HasPermissionAttribute>();
        Assert.NotNull(permissionAttr);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{expectedPermission}", permissionAttr.Policy);
    }
}