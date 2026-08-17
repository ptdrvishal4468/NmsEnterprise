namespace Nms.Domain.Entities;

using Nms.Domain.Common;

public class Site : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? StateOrProvince { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Country { get; private set; }
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    public string? TimeZone { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation
    public ICollection<Building> Buildings { get; private set; } = new List<Building>();

    // EF Core constructor
    protected Site() : base() { }

    public Site(
        Guid id,
        Guid tenantId,
        string name,
        string code,
        string? description = null,
        string? address = null,
        string? city = null,
        string? stateOrProvince = null,
        string? postalCode = null,
        string? country = null,
        double? latitude = null,
        double? longitude = null,
        string? timeZone = null) : base(id)
    {
        TenantId = tenantId;
        Update(name, code, description, address, city, stateOrProvince, postalCode, country, latitude, longitude, timeZone);
    }

    public void Update(
        string name,
        string code,
        string? description,
        string? address,
        string? city,
        string? stateOrProvince,
        string? postalCode,
        string? country,
        double? latitude,
        double? longitude,
        string? timeZone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        Description = description?.Trim();
        Address = address?.Trim();
        City = city?.Trim();
        StateOrProvince = stateOrProvince?.Trim();
        PostalCode = postalCode?.Trim();
        Country = country?.Trim();
        Latitude = latitude;
        Longitude = longitude;
        TimeZone = timeZone?.Trim();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}