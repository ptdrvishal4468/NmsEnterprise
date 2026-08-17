using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Api.Controllers;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;
using Xunit;

namespace Nms.IntegrationTests.Sites;

public class SitesEndpointTests
{
    [Fact]
    public void SitesController_HasAuthorizeAndRouteAttributes()
    {
        // Arrange
        var controllerType = typeof(SitesController);

        // Act
        var routeAttr = Attribute.GetCustomAttribute(controllerType, typeof(RouteAttribute)) as RouteAttribute;
        var authAttr = Attribute.GetCustomAttribute(controllerType, typeof(AuthorizeAttribute)) as AuthorizeAttribute;

        // Assert
        Assert.NotNull(routeAttr);
        Assert.Equal("api/v1/sites", routeAttr.Template);
        Assert.NotNull(authAttr);
    }

    [Theory]
    [InlineData("GetSitesPaged", Permissions.Sites.View)]
    [InlineData("GetSiteById", Permissions.Sites.View)]
    [InlineData("CreateSite", Permissions.Sites.Create)]
    [InlineData("UpdateSite", Permissions.Sites.Update)]
    [InlineData("DeleteSite", Permissions.Sites.Delete)]
    public void SitesController_MethodsHaveCorrectPermissionAttributes(string methodName, string expectedPermission)
    {
        // Arrange
        var method = typeof(SitesController).GetMethod(methodName);
        Assert.NotNull(method);

        // Act
        var permissionAttr = Attribute.GetCustomAttribute(method, typeof(HasPermissionAttribute)) as HasPermissionAttribute;

        // Assert
        Assert.NotNull(permissionAttr);
        Assert.Equal($"PERMISSION_{expectedPermission}", permissionAttr.Policy);
    }
}