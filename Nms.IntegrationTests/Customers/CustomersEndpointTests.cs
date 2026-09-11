using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nms.Api.Controllers;
using Nms.Domain.Constants;
using Nms.Infrastructure.Security;
using Xunit;

namespace Nms.IntegrationTests.Customers;

public class CustomersEndpointTests
{
    [Fact]
    public void CustomersController_HasAuthorizeAndRouteAttributes()
    {
        // Arrange
        var controllerType = typeof(CustomersController);

        // Act
        var routeAttr = Attribute.GetCustomAttribute(controllerType, typeof(RouteAttribute)) as RouteAttribute;
        var authAttr = Attribute.GetCustomAttribute(controllerType, typeof(AuthorizeAttribute)) as AuthorizeAttribute;

        // Assert
        Assert.NotNull(routeAttr);
        Assert.Equal("api/v1/customers", routeAttr.Template);
        Assert.NotNull(authAttr);
    }

    [Theory]
    [InlineData("GetCustomersPaged", Permissions.Customers.View)]
    [InlineData("GetCustomerById", Permissions.Customers.View)]
    [InlineData("GetCustomerHierarchy", Permissions.Customers.View)]
    [InlineData("CreateCustomer", Permissions.Customers.Create)]
    [InlineData("UpdateCustomer", Permissions.Customers.Update)]
    [InlineData("DeleteCustomer", Permissions.Customers.Delete)]
    [InlineData("GetCustomerContacts", Permissions.Customers.View)]
    [InlineData("AddCustomerContact", Permissions.Customers.Create)]
    [InlineData("UpdateCustomerContact", Permissions.Customers.Update)]
    [InlineData("DeleteCustomerContact", Permissions.Customers.Delete)]
    public void CustomersController_MethodsHaveCorrectPermissionAttributes(string methodName, string expectedPermission)
    {
        // Arrange
        var method = typeof(CustomersController).GetMethod(methodName);
        Assert.NotNull(method);

        // Act
        var permissionAttr = Attribute.GetCustomAttribute(method, typeof(HasPermissionAttribute)) as HasPermissionAttribute;

        // Assert
        Assert.NotNull(permissionAttr);
        Assert.Equal($"PERMISSION_{expectedPermission}", permissionAttr.Policy);
    }
}