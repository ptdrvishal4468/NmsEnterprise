using Nms.Domain.Enums;

namespace Nms.Application.Customers.Dtos;

public class CustomerDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? AccountNumber { get; set; }
    public string? Description { get; set; }
    public CustomerStatus Status { get; set; }
    public CustomerTier Tier { get; set; }
    public Guid? ParentCustomerId { get; set; }
    public string? ParentCustomerName { get; set; }
    public string? MetadataJson { get; set; }
    public int ContactCount { get; set; }
    public int ChildCustomerCount { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAtUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}