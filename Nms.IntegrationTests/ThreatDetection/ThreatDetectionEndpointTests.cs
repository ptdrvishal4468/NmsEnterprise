using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Nms.Api.Controllers;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;
using Xunit;

namespace Nms.IntegrationTests.ThreatDetection;

public class ThreatDetectionEndpointTests
{
    [Fact]
    public void ThreatDetectionController_ShouldRequireAuthorizeAttribute()
    {
        var type = typeof(ThreatDetectionController);
        var authAttribute = type.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authAttribute);
    }

    [Theory]
    [InlineData(nameof(ThreatDetectionController.GetIndicators), Permissions.ThreatDetection.View)]
    [InlineData(nameof(ThreatDetectionController.GetIndicatorById), Permissions.ThreatDetection.View)]
    [InlineData(nameof(ThreatDetectionController.UpdateIndicatorStatus), Permissions.ThreatDetection.Manage)]
    [InlineData(nameof(ThreatDetectionController.AnalyzeFailedLogins), Permissions.ThreatDetection.Analyze)]
    [InlineData(nameof(ThreatDetectionController.AnalyzeConfigDrift), Permissions.ThreatDetection.Analyze)]
    [InlineData(nameof(ThreatDetectionController.GetConfigDrifts), Permissions.ThreatDetection.View)]
    [InlineData(nameof(ThreatDetectionController.AnalyzePortScans), Permissions.ThreatDetection.Analyze)]
    [InlineData(nameof(ThreatDetectionController.AnalyzeUnauthorizedAccess), Permissions.ThreatDetection.Analyze)]
    [InlineData(nameof(ThreatDetectionController.GetSummary), Permissions.ThreatDetection.View)]
    [InlineData(nameof(ThreatDetectionController.GetRules), Permissions.ThreatDetection.View)]
    [InlineData(nameof(ThreatDetectionController.CreateRule), Permissions.ThreatDetection.Manage)]
    public void ThreatDetectionController_Endpoints_ShouldEnforceDynamicRBACPermissions(string methodName, string expectedPermission)
    {
        var method = typeof(ThreatDetectionController).GetMethod(methodName);
        Assert.NotNull(method);

        var attribute = method.GetCustomAttribute<HasPermissionAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{expectedPermission}", attribute.Policy);
    }
}