using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests.Assets;

public class AssetDomainTests
{
    [Fact]
    public void Constructor_WithValidArguments_InitializesCorrectly()
    {
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var asset = new Asset(
            id: id,
            tenantId: tenantId,
            assetTag: "AST-1001",
            name: "Core Switch Alpha",
            lifecycleState: AssetLifecycleState.InStock,
            serialNumber: "SN-998811",
            vendor: "Cisco",
            model: "Catalyst 9300",
            category: "Switch",
            siteOrLocation: "Bhopal DC-1",
            rackIdentifier: "Rack-01",
            rackUnitPosition: "U10-U11",
            department: "Network Engineering",
            purchasePrice: 4500.00m,
            currency: "USD");

        Assert.Equal(id, asset.Id);
        Assert.Equal(tenantId, asset.TenantId);
        Assert.Equal("AST-1001", asset.AssetTag);
        Assert.Equal("Core Switch Alpha", asset.Name);
        Assert.Equal(AssetLifecycleState.InStock, asset.LifecycleState);
        Assert.Equal("SN-998811", asset.SerialNumber);
        Assert.Equal("Cisco", asset.Vendor);
        Assert.Equal("Catalyst 9300", asset.Model);
        Assert.Equal("Switch", asset.Category);
        Assert.Equal("Bhopal DC-1", asset.SiteOrLocation);
        Assert.Equal("Rack-01", asset.RackIdentifier);
        Assert.Equal("U10-U11", asset.RackUnitPosition);
        Assert.Equal("Network Engineering", asset.Department);
        Assert.Equal(4500.00m, asset.PurchasePrice);
        Assert.Equal("USD", asset.Currency);
        Assert.NotNull(asset.LifecycleChangedAtUtc);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithInvalidAssetTag_ThrowsArgumentException(string? invalidTag)
    {
        Assert.Throws<ArgumentException>(() => new Asset(
            id: Guid.NewGuid(),
            tenantId: Guid.NewGuid(),
            assetTag: invalidTag!,
            name: "Router Gateway"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithInvalidName_ThrowsArgumentException(string? invalidName)
    {
        Assert.Throws<ArgumentException>(() => new Asset(
            id: Guid.NewGuid(),
            tenantId: Guid.NewGuid(),
            assetTag: "AST-2001",
            name: invalidName!));
    }

    [Fact]
    public void Constructor_WithNegativePrice_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Asset(
            id: Guid.NewGuid(),
            tenantId: Guid.NewGuid(),
            assetTag: "AST-2002",
            name: "Firewall Node",
            purchasePrice: -100.00m));
    }

    [Fact]
    public void ChangeLifecycleState_UpdatesStateNotesAndTimestamp()
    {
        var asset = new Asset(
            id: Guid.NewGuid(),
            tenantId: Guid.NewGuid(),
            assetTag: "AST-3001",
            name: "Server Rack Alpha",
            lifecycleState: AssetLifecycleState.InStock);

        var transitionTimeBefore = DateTime.UtcNow;
        asset.ChangeLifecycleState(AssetLifecycleState.InService, "Deployed to primary rack cluster.");

        Assert.Equal(AssetLifecycleState.InService, asset.LifecycleState);
        Assert.Equal("Deployed to primary rack cluster.", asset.LifecycleNotes);
        Assert.True(asset.LifecycleChangedAtUtc >= transitionTimeBefore);
    }

    [Fact]
    public void UpdateDetails_WithValidData_UpdatesAllFields()
    {
        var asset = new Asset(
            id: Guid.NewGuid(),
            tenantId: Guid.NewGuid(),
            assetTag: "AST-4001",
            name: "Old Name");

        asset.UpdateDetails(
            name: "Updated Name",
            serialNumber: "SN-NEW",
            vendor: "Arista",
            model: "7050SX",
            category: "Top-of-Rack Switch",
            siteOrLocation: "Bhopal DC-2",
            rackIdentifier: "Rack-04",
            rackUnitPosition: "U01",
            department: "Infrastructure",
            assignedToUserId: null,
            deviceId: null,
            warrantyProvider: "Arista Care",
            warrantyStartDateUtc: DateTime.UtcNow,
            warrantyEndDateUtc: DateTime.UtcNow.AddYears(3),
            warrantyStatus: WarrantyStatus.Active,
            warrantyContractNumber: "WAR-ARISTA-99",
            purchaseDateUtc: DateTime.UtcNow.AddMonths(-1),
            purchaseOrderNumber: "PO-7788",
            purchasePrice: 6200.50m,
            currency: "USD");

        Assert.Equal("Updated Name", asset.Name);
        Assert.Equal("SN-NEW", asset.SerialNumber);
        Assert.Equal("Arista", asset.Vendor);
        Assert.Equal(WarrantyStatus.Active, asset.WarrantyStatus);
        Assert.Equal(6200.50m, asset.PurchasePrice);
    }
}