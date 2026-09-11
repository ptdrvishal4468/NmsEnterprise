using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Api.Controllers;
using Nms.Infrastructure.Security;
using Xunit;

namespace Nms.IntegrationTests.Topology;

public class TopologyEndpointTests
{
    [Fact]
    public void TopologyController_ShouldHaveAuthorizeAndRouteAttributes()
    {
        var type = typeof(TopologyController);

        var authAttr = type.GetCustomAttribute<AuthorizeAttribute>();
        var routeAttr = type.GetCustomAttribute<RouteAttribute>();
        var apiControllerAttr = type.GetCustomAttribute<ApiControllerAttribute>();

        Assert.NotNull(authAttr);
        Assert.NotNull(routeAttr);
        Assert.NotNull(apiControllerAttr);
        Assert.Equal("api/v1/[controller]", routeAttr.Template);
    }

    [Fact]
    public void GetTopologyGraph_ShouldHaveHasPermissionAttribute()
    {
        var method = typeof(TopologyController).GetMethod(nameof(TopologyController.GetTopologyGraph));
        Assert.NotNull(method);

        var permAttr = method.GetCustomAttribute<HasPermissionAttribute>();
        Assert.NotNull(permAttr);
    }
}