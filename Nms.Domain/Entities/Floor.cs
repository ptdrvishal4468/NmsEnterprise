namespace Nms.Domain.Entities;

using Nms.Domain.Common;

public class Floor : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid BuildingId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int FloorNumber { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigations
    public Building Building { get; private set; } = null!;
    public ICollection<Room> Rooms { get; private set; } = new List<Room>();

    protected Floor() : base() { }

    public Floor(
        Guid id,
        Guid tenantId,
        Guid buildingId,
        string name,
        int floorNumber,
        string? description = null) : base(id)
    {
        TenantId = tenantId;
        BuildingId = buildingId;
        Update(name, floorNumber, description);
    }

    public void Update(string name, int floorNumber, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name.Trim();
        FloorNumber = floorNumber;
        Description = description?.Trim();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}