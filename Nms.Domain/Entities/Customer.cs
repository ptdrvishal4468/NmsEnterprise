using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class Customer : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string? AccountNumber { get; private set; }
    public string? Description { get; private set; }
    public CustomerStatus Status { get; private set; } = CustomerStatus.Active;
    public CustomerTier Tier { get; private set; } = CustomerTier.Standard;
    public string? MetadataJson { get; private set; }

    // Hierarchy Navigation
    public Guid? ParentCustomerId { get; private set; }
    public Customer? ParentCustomer { get; private set; }
    public ICollection<Customer> ChildCustomers { get; private set; } = new List<Customer>();

    // Contacts Navigation
    public ICollection<CustomerContact> Contacts { get; private set; } = new List<CustomerContact>();

    private Customer() { }

    public Customer(
        Guid id,
        Guid tenantId,
        string name,
        string code,
        string? accountNumber = null,
        string? description = null,
        CustomerStatus status = CustomerStatus.Active,
        CustomerTier tier = CustomerTier.Standard,
        Guid? parentCustomerId = null,
        string? metadataJson = null) : base(id)
    {
        TenantId = tenantId;
        Name = name;
        Code = code;
        AccountNumber = accountNumber;
        Description = description;
        Status = status;
        Tier = tier;
        ParentCustomerId = parentCustomerId;
        MetadataJson = metadataJson;
    }

    public void UpdateDetails(
        string name,
        string code,
        string? accountNumber,
        string? description,
        CustomerStatus status,
        CustomerTier tier,
        Guid? parentCustomerId,
        string? metadataJson)
    {
        if (parentCustomerId.HasValue && parentCustomerId.Value == Id)
        {
            throw aerial_invalid_operation();
        }

        Name = name;
        Code = code;
        AccountNumber = accountNumber;
        Description = description;
        Status = status;
        Tier = tier;
        ParentCustomerId = parentCustomerId;
        MetadataJson = metadataJson;
    }

    public void SetParentCustomer(Guid? parentCustomerId)
    {
        if (parentCustomerId.HasValue && parentCustomerId.Value == Id)
        {
            throw aerial_invalid_operation();
        }

        ParentCustomerId = parentCustomerId;
    }

    public void SetStatus(CustomerStatus status)
    {
        Status = status;
    }

    public void SetMetadata(string? metadataJson)
    {
        MetadataJson = metadataJson;
    }

    private static InvalidOperationException aerial_invalid_operation() =>
        new("A customer cannot be assigned as its own parent.");
}