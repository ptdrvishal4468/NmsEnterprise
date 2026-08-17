namespace Nms.Domain.Entities;

using Nms.Domain.Common;

public class Building : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid SiteId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Address { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigations
    public Site Site { get; private set; } = null!;
    public ICollection<Floor> Floors { get; private set; } = new List<Floor>();

    protected Building() : base() { }

    public Building(
        Guid id,
        Guid tenantId,
        Guid siteId,
        string name,
        string code,
        string? description = null,
        string? address = null) : base(id)
    {
        TenantId = tenantId;
        SiteId = siteId;
        Update(name, code, description, address);
    }

    public void Update(string name, string code, string? description, string? address)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        Description = description?.Trim();
        Address = address?.Trim();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}