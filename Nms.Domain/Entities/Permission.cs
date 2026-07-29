using Nms.Domain.Common;

namespace Nms.Domain.Entities;

public class Permission : BaseEntity<int>
{
    public string PermissionKey { get; private set; } = string.Empty; // Format: Module.Action (e.g., 'Inventory.Read')
    public string? Description { get; private set; }

    private Permission() { }

    public Permission(int id, string permissionKey, string? description = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(permissionKey))
            throw new ArgumentException("Permission key cannot be empty.", nameof(permissionKey));

        PermissionKey = permissionKey;
        Description = description;
    }
}