using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class CustomerContact : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; private set; }
    public Customer? Customer { get; private set; }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public string? JobTitle { get; private set; }
    public ContactType ContactType { get; private set; } = ContactType.Technical;
    public bool IsPrimary { get; private set; }

    private CustomerContact() { }

    public CustomerContact(
        Guid id,
        Guid tenantId,
        Guid customerId,
        string firstName,
        string lastName,
        string email,
        string? phoneNumber = null,
        string? jobTitle = null,
        ContactType contactType = ContactType.Technical,
        bool isPrimary = false) : base(id)
    {
        TenantId = tenantId;
        CustomerId = customerId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        JobTitle = jobTitle;
        ContactType = contactType;
        IsPrimary = isPrimary;
    }

    public void UpdateContactDetails(
        string firstName,
        string lastName,
        string email,
        string? phoneNumber,
        string? jobTitle,
        ContactType contactType,
        bool isPrimary)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        JobTitle = jobTitle;
        ContactType = contactType;
        IsPrimary = isPrimary;
    }

    public void SetPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }
}