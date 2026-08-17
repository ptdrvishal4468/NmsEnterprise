namespace Nms.Domain.Entities;

using Nms.Domain.Common;

public class Room : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid FloorId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string? RoomType { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigations
    public Floor Floor { get; private set; } = null!;
    public ICollection<Rack> Racks { get; private set; } = new List<Rack>();

    protected Room() : base() { }

    public Room(
        Guid id,
        Guid tenantId,
        Guid floorId,
        string name,
        string code,
        string? roomType = null,
        string? description = null) : base(id)
    {
        TenantId = tenantId;
        FloorId = floorId;
        Update(name, code, roomType, description);
    }

    public void Update(string name, string code, string? roomType, string? description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        RoomType = roomType?.Trim();
        Description = description?.Trim();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}