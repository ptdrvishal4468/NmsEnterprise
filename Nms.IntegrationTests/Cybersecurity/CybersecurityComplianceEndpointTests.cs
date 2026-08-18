using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Api.Controllers;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;
using Xunit;

namespace Nms.IntegrationTests.Cybersecurity;

public class CybersecurityComplianceEndpointTests
{
    [Fact]
    public void CybersecurityComplianceController_HasApiControllerAndAuthorizeAttributes()
    {
        var type = typeof(CybersecurityComplianceController);

        var apiControllerAttr = type.GetCustomAttribute<ApiControllerAttribute>();
        Assert.NotNull(apiControllerAttr);

        var authorizeAttr = type.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authorizeAttr);

        var routeAttr = type.GetCustomAttribute<RouteAttribute>();
        Assert.NotNull(routeAttr);
        Assert.Equal("api/v1/cybersecurity", routeAttr!.Template);
    }

    [Theory]
    [InlineData(nameof(CybersecurityComplianceController.GetPolicies))]
    [InlineData(nameof(CybersecurityComplianceController.GetPolicyById))]
    [InlineData(nameof(CybersecurityComplianceController.CreatePolicy))]
    [InlineData(nameof(CybersecurityComplianceController.UpdatePolicy))]
    [InlineData(nameof(CybersecurityComplianceController.DeletePolicy))]
    [InlineData(nameof(CybersecurityComplianceController.EvaluateDevice))]
    [InlineData(nameof(CybersecurityComplianceController.EvaluateAllDevices))]
    [InlineData(nameof(CybersecurityComplianceController.GetScans))]
    [InlineData(nameof(CybersecurityComplianceController.GetScanById))]
    [InlineData(nameof(CybersecurityComplianceController.GetDeviceLatestScan))]
    [InlineData(nameof(CybersecurityComplianceController.GetPostureSummary))]
    public void EndpointActions_AreDecoratedWithHasPermissionAttribute(string actionName)
    {
        var method = typeof(CybersecurityComplianceController).GetMethod(actionName);
        Assert.NotNull(method);

        var permissionAttr = method!.GetCustomAttribute<HasPermissionAttribute>();
        Assert.NotNull(permissionAttr);
    }
}