using Nms.Domain.Enums;

namespace Nms.Application.Customers.Dtos;

public class UpdateCustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? AccountNumber { get; set; }
    public string? Description { get; set; }
    public CustomerStatus Status { get; set; }
    public CustomerTier Tier { get; set; }
    public Guid? ParentCustomerId { get; set; }
    public string? MetadataJson { get; set; }
}