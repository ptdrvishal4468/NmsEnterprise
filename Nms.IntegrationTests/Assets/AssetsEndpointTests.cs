using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Api.Controllers;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;
using Xunit;

namespace Nms.IntegrationTests.Assets;

public class AssetsEndpointTests
{
    [Fact]
    public void AssetsController_HasAuthorizeAttributeAndRoute()
    {
        var type = typeof(AssetsController);

        var authorizeAttr = type.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authorizeAttr);

        var routeAttr = type.GetCustomAttribute<RouteAttribute>();
        Assert.NotNull(routeAttr);
        Assert.Equal("api/v1/assets", routeAttr.Template);
    }

    [Theory]
    [InlineData(nameof(AssetsController.GetAssetsPaged), Permissions.Assets.View)]
    [InlineData(nameof(AssetsController.GetAssetById), Permissions.Assets.View)]
    [InlineData(nameof(AssetsController.CreateAsset), Permissions.Assets.Create)]
    [InlineData(nameof(AssetsController.UpdateAsset), Permissions.Assets.Update)]
    [InlineData(nameof(AssetsController.ChangeLifecycle), Permissions.Assets.ManageLifecycle)]
    [InlineData(nameof(AssetsController.DeleteAsset), Permissions.Assets.Delete)]
    public void AssetsController_Endpoints_HaveExactPermissionAttributes(string methodName, string expectedPermission)
    {
        var type = typeof(AssetsController);
        var method = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                         .FirstOrDefault(m => m.Name == methodName);

        Assert.NotNull(method);

        var permissionAttr = method.GetCustomAttribute<HasPermissionAttribute>();
        Assert.NotNull(permissionAttr);
        Assert.Equal($"{HasPermissionAttribute.PolicyPrefix}{expectedPermission}", permissionAttr.Policy);
    }
}