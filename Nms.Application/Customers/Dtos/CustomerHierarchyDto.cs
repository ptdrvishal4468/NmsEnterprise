using Nms.Domain.Enums;

namespace Nms.Application.Customers.Dtos;

public class CustomerHierarchyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public CustomerStatus Status { get; set; }
    public CustomerTier Tier { get; set; }
    public Guid? ParentCustomerId { get; set; }
    public IReadOnlyList<CustomerHierarchyDto> Children { get; set; } = new List<CustomerHierarchyDto>();
}