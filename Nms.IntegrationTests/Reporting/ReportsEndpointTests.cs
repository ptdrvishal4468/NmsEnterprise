using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Api.Controllers;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;
using Xunit;

namespace Nms.IntegrationTests.Reporting;

public class ReportsEndpointTests
{
    [Fact]
    public void ReportsController_ShouldHaveAuthorizeAndRouteAttributes()
    {
        var type = typeof(ReportsController);

        var authAttr = type.GetCustomAttribute<AuthorizeAttribute>();
        var routeAttr = type.GetCustomAttribute<RouteAttribute>();
        var apiControllerAttr = type.GetCustomAttribute<ApiControllerAttribute>();

        Assert.NotNull(authAttr);
        Assert.NotNull(routeAttr);
        Assert.NotNull(apiControllerAttr);
        Assert.Equal("api/v1/[controller]", routeAttr.Template);
    }

    [Fact]
    public void GenerateDeviceReport_ShouldHaveExpectedAttributes()
    {
        var method = typeof(ReportsController).GetMethod(nameof(ReportsController.GenerateDeviceReport));
        Assert.NotNull(method);

        var httpGetAttr = method.GetCustomAttribute<HttpGetAttribute>();
        var permAttr = method.GetCustomAttribute<HasPermissionAttribute>();

        Assert.NotNull(httpGetAttr);
        Assert.Equal("devices", httpGetAttr.Template);
        Assert.NotNull(permAttr);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{Permissions.Reports.Generate}", permAttr.Policy);
    }

    [Fact]
    public void GenerateHealthReport_ShouldHaveExpectedAttributes()
    {
        var method = typeof(ReportsController).GetMethod(nameof(ReportsController.GenerateHealthReport));
        Assert.NotNull(method);

        var httpGetAttr = method.GetCustomAttribute<HttpGetAttribute>();
        var permAttr = method.GetCustomAttribute<HasPermissionAttribute>();

        Assert.NotNull(httpGetAttr);
        Assert.Equal("health", httpGetAttr.Template);
        Assert.NotNull(permAttr);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{Permissions.Reports.Generate}", permAttr.Policy);
    }

    [Fact]
    public void GenerateInventoryReport_ShouldHaveExpectedAttributes()
    {
        var method = typeof(ReportsController).GetMethod(nameof(ReportsController.GenerateInventoryReport));
        Assert.NotNull(method);

        var httpGetAttr = method.GetCustomAttribute<HttpGetAttribute>();
        var permAttr = method.GetCustomAttribute<HasPermissionAttribute>();

        Assert.NotNull(httpGetAttr);
        Assert.Equal("inventory", httpGetAttr.Template);
        Assert.NotNull(permAttr);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{Permissions.Reports.Generate}", permAttr.Policy);
    }

    [Fact]
    public void GenerateAlertReport_ShouldHaveExpectedAttributes()
    {
        var method = typeof(ReportsController).GetMethod(nameof(ReportsController.GenerateAlertReport));
        Assert.NotNull(method);

        var httpGetAttr = method.GetCustomAttribute<HttpGetAttribute>();
        var permAttr = method.GetCustomAttribute<HasPermissionAttribute>();

        Assert.NotNull(httpGetAttr);
        Assert.Equal("alerts", httpGetAttr.Template);
        Assert.NotNull(permAttr);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{Permissions.Reports.Generate}", permAttr.Policy);
    }

    [Fact]
    public void CreateScheduledReport_ShouldHaveExpectedAttributes()
    {
        var method = typeof(ReportsController).GetMethod(nameof(ReportsController.CreateScheduledReport));
        Assert.NotNull(method);

        var httpPostAttr = method.GetCustomAttribute<HttpPostAttribute>();
        var permAttr = method.GetCustomAttribute<HasPermissionAttribute>();

        Assert.NotNull(httpPostAttr);
        Assert.Equal("schedules", httpPostAttr.Template);
        Assert.NotNull(permAttr);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{Permissions.Reports.ManageSchedules}", permAttr.Policy);
    }
}