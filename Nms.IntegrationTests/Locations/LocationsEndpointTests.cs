using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Api.Controllers;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;
using Xunit;

namespace Nms.IntegrationTests.Locations;

public class LocationsEndpointTests
{
    [Theory]
    [InlineData(typeof(BuildingsController), "api/v1/buildings")]
    [InlineData(typeof(FloorsController), "api/v1/floors")]
    [InlineData(typeof(RoomsController), "api/v1/rooms")]
    [InlineData(typeof(RacksController), "api/v1/racks")]
    public void LocationControllers_HaveAuthorizeAndRouteAttributes(Type controllerType, string expectedRoute)
    {
        // Act
        var routeAttr = Attribute.GetCustomAttribute(controllerType, typeof(RouteAttribute)) as RouteAttribute;
        var authAttr = Attribute.GetCustomAttribute(controllerType, typeof(AuthorizeAttribute)) as AuthorizeAttribute;

        // Assert
        Assert.NotNull(routeAttr);
        Assert.Equal(expectedRoute, routeAttr.Template);
        Assert.NotNull(authAttr);
    }

    [Theory]
    [InlineData(typeof(BuildingsController), "GetBuildingsPaged", Permissions.Locations.View)]
    [InlineData(typeof(BuildingsController), "CreateBuilding", Permissions.Locations.Create)]
    [InlineData(typeof(FloorsController), "GetFloorsPaged", Permissions.Locations.View)]
    [InlineData(typeof(FloorsController), "CreateFloor", Permissions.Locations.Create)]
    [InlineData(typeof(RoomsController), "GetRoomsPaged", Permissions.Locations.View)]
    [InlineData(typeof(RoomsController), "CreateRoom", Permissions.Locations.Create)]
    [InlineData(typeof(RacksController), "GetRacksPaged", Permissions.Locations.View)]
    [InlineData(typeof(RacksController), "CreateRack", Permissions.Locations.Create)]
    public void LocationControllers_MethodsHaveCorrectPermissionAttributes(Type controllerType, string methodName, string expectedPermission)
    {
        // Arrange
        var method = controllerType.GetMethod(methodName);
        Assert.NotNull(method);

        // Act
        var permissionAttr = Attribute.GetCustomAttribute(method, typeof(HasPermissionAttribute)) as HasPermissionAttribute;

        // Assert
        Assert.NotNull(permissionAttr);
        Assert.Equal($"PERMISSION_{expectedPermission}", permissionAttr.Policy);
    }
}