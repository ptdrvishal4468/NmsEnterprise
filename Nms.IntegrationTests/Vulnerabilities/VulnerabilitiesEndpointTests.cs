using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Nms.Api.Controllers;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;
using Xunit;

namespace Nms.IntegrationTests.Vulnerabilities;

public class VulnerabilitiesEndpointTests
{
    [Fact]
    public void VulnerabilitiesController_HasAuthorizeAttribute()
    {
        var controllerType = typeof(VulnerabilitiesController);
        var authAttr = controllerType.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authAttr);
    }

    [Theory]
    [InlineData(nameof(VulnerabilitiesController.GetVulnerabilities), Permissions.Vulnerabilities.View)]
    [InlineData(nameof(VulnerabilitiesController.GetVulnerabilityById), Permissions.Vulnerabilities.View)]
    [InlineData(nameof(VulnerabilitiesController.CreateVulnerability), Permissions.Vulnerabilities.Manage)]
    [InlineData(nameof(VulnerabilitiesController.GetSecurityAdvisories), Permissions.Vulnerabilities.View)]
    [InlineData(nameof(VulnerabilitiesController.GetSecurityAdvisoryById), Permissions.Vulnerabilities.View)]
    [InlineData(nameof(VulnerabilitiesController.CreateSecurityAdvisory), Permissions.Vulnerabilities.Manage)]
    [InlineData(nameof(VulnerabilitiesController.AnalyzeDevice), Permissions.Vulnerabilities.Analyze)]
    [InlineData(nameof(VulnerabilitiesController.AnalyzeAllDevices), Permissions.Vulnerabilities.Analyze)]
    [InlineData(nameof(VulnerabilitiesController.GetDeviceVulnerabilityMatches), Permissions.Vulnerabilities.View)]
    [InlineData(nameof(VulnerabilitiesController.UpdateMatchStatus), Permissions.Vulnerabilities.Manage)]
    [InlineData(nameof(VulnerabilitiesController.GenerateUpgradeRecommendations), Permissions.Vulnerabilities.Recommendations)]
    [InlineData(nameof(VulnerabilitiesController.GetUpgradeRecommendations), Permissions.Vulnerabilities.View)]
    [InlineData(nameof(VulnerabilitiesController.GetFirmwareRiskSummary), Permissions.Vulnerabilities.View)]
    public void Endpoint_EnforcesRequiredPermission(string methodName, string expectedPermission)
    {
        var method = typeof(VulnerabilitiesController).GetMethod(methodName);
        Assert.NotNull(method);

        var permAttr = method.GetCustomAttribute<HasPermissionAttribute>();
        Assert.NotNull(permAttr);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{expectedPermission}", permAttr.Policy);
    }
}