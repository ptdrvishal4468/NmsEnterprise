using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests.Customers;

public class CustomerDomainTests
{
    [Fact]
    public void Constructor_ValidArguments_InitializesPropertiesCorrectly()
    {
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var parentId = Guid.NewGuid();

        var customer = new Customer(
            id: id,
            tenantId: tenantId,
            name: "Acme Corp",
            code: "ACME-01",
            accountNumber: "ACC-9988",
            description: "Enterprise Account",
            status: CustomerStatus.Active,
            tier: CustomerTier.Enterprise,
            parentCustomerId: parentId,
            metadataJson: "{\"sla\":\"platinum\"}");

        Assert.Equal(id, customer.Id);
        Assert.Equal(tenantId, customer.TenantId);
        Assert.Equal("Acme Corp", customer.Name);
        Assert.Equal("ACME-01", customer.Code);
        Assert.Equal("ACC-9988", customer.AccountNumber);
        Assert.Equal("Enterprise Account", customer.Description);
        Assert.Equal(CustomerStatus.Active, customer.Status);
        Assert.Equal(CustomerTier.Enterprise, customer.Tier);
        Assert.Equal(parentId, customer.ParentCustomerId);
        Assert.Equal("{\"sla\":\"platinum\"}", customer.MetadataJson);
    }

    [Fact]
    public void UpdateDetails_SelfAsParent_ThrowsInvalidOperationException()
    {
        var id = Guid.NewGuid();
        var customer = new Customer(id, Guid.NewGuid(), "Test Corp", "TEST-01");

        Assert.Throws<InvalidOperationException>(() =>
            customer.UpdateDetails("Test Corp", "TEST-01", null, null, CustomerStatus.Active, CustomerTier.Standard, id, null));
    }

    [Fact]
    public void SetParentCustomer_SelfAsParent_ThrowsInvalidOperationException()
    {
        var id = Guid.NewGuid();
        var customer = new Customer(id, Guid.NewGuid(), "Test Corp", "TEST-01");

        Assert.Throws<InvalidOperationException>(() =>
            customer.SetParentCustomer(id));
    }

    [Fact]
    public void CustomerContact_SetPrimary_UpdatesStatus()
    {
        var contact = new CustomerContact(
            id: Guid.NewGuid(),
            tenantId: Guid.NewGuid(),
            customerId: Guid.NewGuid(),
            firstName: "John",
            lastName: "Doe",
            email: "john.doe@example.com",
            isPrimary: false);

        contact.SetPrimary(true);

        Assert.True(contact.IsPrimary);
    }
}