using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Api.Controllers;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;
using Xunit;

namespace Nms.IntegrationTests.Dashboard;

public class DashboardEndpointTests
{
    [Fact]
    public void DashboardController_ShouldHaveAuthorizeAndRouteAttributes()
    {
        var type = typeof(DashboardController);

        var authAttr = type.GetCustomAttribute<AuthorizeAttribute>();
        var routeAttr = type.GetCustomAttribute<RouteAttribute>();
        var apiControllerAttr = type.GetCustomAttribute<ApiControllerAttribute>();

        Assert.NotNull(authAttr);
        Assert.NotNull(routeAttr);
        Assert.NotNull(apiControllerAttr);
        Assert.Equal("api/v1/[controller]", routeAttr.Template);
    }

    [Fact]
    public void GetExecutiveDashboard_ShouldHaveExpectedAttributes()
    {
        var method = typeof(DashboardController).GetMethod(nameof(DashboardController.GetExecutiveDashboard));
        Assert.NotNull(method);

        var httpGetAttr = method.GetCustomAttribute<HttpGetAttribute>();
        var permAttr = method.GetCustomAttribute<HasPermissionAttribute>();

        Assert.NotNull(httpGetAttr);
        Assert.Equal("executive", httpGetAttr.Template);
        Assert.NotNull(permAttr);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{Permissions.Dashboard.ViewExecutive}", permAttr.Policy);
    }

    [Fact]
    public void GetDeviceStatistics_ShouldHaveExpectedAttributes()
    {
        var method = typeof(DashboardController).GetMethod(nameof(DashboardController.GetDeviceStatistics));
        Assert.NotNull(method);

        var httpGetAttr = method.GetCustomAttribute<HttpGetAttribute>();
        var permAttr = method.GetCustomAttribute<HasPermissionAttribute>();

        Assert.NotNull(httpGetAttr);
        Assert.Equal("devices", httpGetAttr.Template);
        Assert.NotNull(permAttr);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{Permissions.Dashboard.View}", permAttr.Policy);
    }

    [Fact]
    public void GetHealthSummary_ShouldHaveExpectedAttributes()
    {
        var method = typeof(DashboardController).GetMethod(nameof(DashboardController.GetHealthSummary));
        Assert.NotNull(method);

        var httpGetAttr = method.GetCustomAttribute<HttpGetAttribute>();
        var permAttr = method.GetCustomAttribute<HasPermissionAttribute>();

        Assert.NotNull(httpGetAttr);
        Assert.Equal("health", httpGetAttr.Template);
        Assert.NotNull(permAttr);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{Permissions.Dashboard.View}", permAttr.Policy);
    }

    [Fact]
    public void GetAlertSummary_ShouldHaveExpectedAttributes()
    {
        var method = typeof(DashboardController).GetMethod(nameof(DashboardController.GetAlertSummary));
        Assert.NotNull(method);

        var httpGetAttr = method.GetCustomAttribute<HttpGetAttribute>();
        var permAttr = method.GetCustomAttribute<HasPermissionAttribute>();

        Assert.NotNull(httpGetAttr);
        Assert.Equal("alerts", httpGetAttr.Template);
        Assert.NotNull(permAttr);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{Permissions.Dashboard.View}", permAttr.Policy);
    }

    [Fact]
    public void GetTenantDashboard_ShouldHaveExpectedAttributes()
    {
        var method = typeof(DashboardController).GetMethod(nameof(DashboardController.GetTenantDashboard));
        Assert.NotNull(method);

        var httpGetAttr = method.GetCustomAttribute<HttpGetAttribute>();
        var permAttr = method.GetCustomAttribute<HasPermissionAttribute>();

        Assert.NotNull(httpGetAttr);
        Assert.Equal("tenant", httpGetAttr.Template);
        Assert.NotNull(permAttr);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{Permissions.Dashboard.ViewTenant}", permAttr.Policy);
    }
}