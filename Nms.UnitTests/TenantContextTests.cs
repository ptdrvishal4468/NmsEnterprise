using Nms.Infrastructure.Tenants;
using Xunit;

namespace Nms.UnitTests;

public class TenantContextTests
{
    [Fact]
    public void SetTenantId_ShouldPopulateTenantIdAndSetIsResolvedTrue()
    {
        // Arrange
        var tenantContext = new TenantContext();
        var expectedTenantId = Guid.NewGuid();

        // Act
        tenantContext.SetTenantId(expectedTenantId);

        // Assert
        Assert.True(tenantContext.IsResolved);
        Assert.Equal(expectedTenantId, tenantContext.TenantId);
    }

    [Fact]
    public void UnsetTenantContext_ShouldHaveIsResolvedFalseAndEmptyGuid()
    {
        // Arrange & Act
        var tenantContext = new TenantContext();

        // Assert
        Assert.False(tenantContext.IsResolved);
        Assert.Equal(Guid.Empty, tenantContext.TenantId);
    }
}