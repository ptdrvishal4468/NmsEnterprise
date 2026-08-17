using Nms.Domain.Entities;
using Xunit;

namespace Nms.UnitTests.Sites;

public class SiteDomainTests
{
    [Fact]
    public void Constructor_WithValidParameters_InitializesCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        // Act
        var site = new Site(
            id,
            tenantId,
            "North America DC",
            "NA-DC-01",
            "Primary datacenter",
            "123 Tech Blvd",
            "Dallas",
            "TX",
            "75001",
            "USA",
            32.7767,
            -96.7970,
            "America/Chicago");

        // Assert
        Assert.Equal(id, site.Id);
        Assert.Equal(tenantId, site.TenantId);
        Assert.Equal("North America DC", site.Name);
        Assert.Equal("NA-DC-01", site.Code);
        Assert.Equal("Dallas", site.City);
        Assert.True(site.IsActive);
    }

    [Theory]
    [InlineData("", "CODE")]
    [InlineData("   ", "CODE")]
    [InlineData("Site Name", "")]
    [InlineData("Site Name", "   ")]
    public void Constructor_WithInvalidRequiredFields_ThrowsArgumentException(string name, string code)
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => new Site(
            Guid.NewGuid(),
            Guid.NewGuid(),
            name,
            code));
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        // Arrange
        var site = new Site(Guid.NewGuid(), Guid.NewGuid(), "Main Site", "SITE-01");

        // Act
        site.Deactivate();

        // Assert
        Assert.False(site.IsActive);
    }

    [Fact]
    public void Activate_SetsIsActiveToTrue()
    {
        // Arrange
        var site = new Site(Guid.NewGuid(), Guid.NewGuid(), "Main Site", "SITE-01");
        site.Deactivate();

        // Act
        site.Activate();

        // Assert
        Assert.True(site.IsActive);
    }
}