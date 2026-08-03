using Nms.Domain.Common;

namespace Nms.Domain.Entities;

public class Tenant : BaseEntity<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    // Private constructor for EF Core
    private Tenant() { }

    public Tenant(Guid id, string name) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tenant name cannot be empty.", nameof(name));

        Name = name;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Tenant name cannot be empty.", nameof(newName));

        Name = newName.Trim();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}