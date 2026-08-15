using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Api.Controllers;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;
using Xunit;

namespace Nms.IntegrationTests.Auditing;

public class AuditEndpointTests
{
    [Fact]
    public void AuditController_ShouldHaveAuthorizeAndRouteAttributes()
    {
        var type = typeof(AuditController);

        var authAttr = type.GetCustomAttribute<AuthorizeAttribute>();
        var routeAttr = type.GetCustomAttribute<RouteAttribute>();
        var apiControllerAttr = type.GetCustomAttribute<ApiControllerAttribute>();

        Assert.NotNull(authAttr);
        Assert.NotNull(routeAttr);
        Assert.NotNull(apiControllerAttr);
        Assert.Equal("api/v1/[controller]", routeAttr.Template);
    }

    [Fact]
    public void Search_ShouldHaveExpectedAttributes()
    {
        var method = typeof(AuditController).GetMethod(nameof(AuditController.Search));
        Assert.NotNull(method);

        var httpGetAttr = method.GetCustomAttribute<HttpGetAttribute>();
        var permAttr = method.GetCustomAttribute<HasPermissionAttribute>();

        Assert.NotNull(httpGetAttr);
        Assert.Equal("search", httpGetAttr.Template);
        Assert.NotNull(permAttr);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{Permissions.Audit.Search}", permAttr.Policy);
    }

    [Fact]
    public void GetById_ShouldHaveExpectedAttributes()
    {
        var method = typeof(AuditController).GetMethod(nameof(AuditController.GetById));
        Assert.NotNull(method);

        var httpGetAttr = method.GetCustomAttribute<HttpGetAttribute>();
        var permAttr = method.GetCustomAttribute<HasPermissionAttribute>();

        Assert.NotNull(httpGetAttr);
        Assert.Equal("{id:long}", httpGetAttr.Template);
        Assert.NotNull(permAttr);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{Permissions.Audit.View}", permAttr.Policy);
    }

    [Fact]
    public void GetUserActivity_ShouldHaveExpectedAttributes()
    {
        var method = typeof(AuditController).GetMethod(nameof(AuditController.GetUserActivity));
        Assert.NotNull(method);

        var httpGetAttr = method.GetCustomAttribute<HttpGetAttribute>();
        var permAttr = method.GetCustomAttribute<HasPermissionAttribute>();

        Assert.NotNull(httpGetAttr);
        Assert.Equal("user-activity", httpGetAttr.Template);
        Assert.NotNull(permAttr);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{Permissions.Audit.View}", permAttr.Policy);
    }

    [Fact]
    public void GetConfigurationChanges_ShouldHaveExpectedAttributes()
    {
        var method = typeof(AuditController).GetMethod(nameof(AuditController.GetConfigurationChanges));
        Assert.NotNull(method);

        var httpGetAttr = method.GetCustomAttribute<HttpGetAttribute>();
        var permAttr = method.GetCustomAttribute<HasPermissionAttribute>();

        Assert.NotNull(httpGetAttr);
        Assert.Equal("configuration-changes", httpGetAttr.Template);
        Assert.NotNull(permAttr);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{Permissions.Audit.View}", permAttr.Policy);
    }

    [Fact]
    public void GenerateComplianceReport_ShouldHaveExpectedAttributes()
    {
        var method = typeof(AuditController).GetMethod(nameof(AuditController.GenerateComplianceReport));
        Assert.NotNull(method);

        var httpGetAttr = method.GetCustomAttribute<HttpGetAttribute>();
        var permAttr = method.GetCustomAttribute<HasPermissionAttribute>();

        Assert.NotNull(httpGetAttr);
        Assert.Equal("compliance-report", httpGetAttr.Template);
        Assert.NotNull(permAttr);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{Permissions.Audit.ComplianceReport}", permAttr.Policy);
    }

    [Fact]
    public void Record_ShouldHaveExpectedAttributes()
    {
        var method = typeof(AuditController).GetMethod(nameof(AuditController.Record));
        Assert.NotNull(method);

        var httpPostAttr = method.GetCustomAttribute<HttpPostAttribute>();
        var permAttr = method.GetCustomAttribute<HasPermissionAttribute>();

        Assert.NotNull(httpPostAttr);
        Assert.Equal("record", httpPostAttr.Template);
        Assert.NotNull(permAttr);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{Permissions.Audit.Search}", permAttr.Policy);
    }
}