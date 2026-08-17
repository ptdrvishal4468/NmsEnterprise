using Nms.Domain.Enums;

namespace Nms.Application.Customers.Dtos;

public class CustomerContactInputDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }
    public ContactType ContactType { get; set; } = ContactType.Technical;
    public bool IsPrimary { get; set; }
}