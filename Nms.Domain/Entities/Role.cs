using Nms.Domain.Common;

namespace Nms.Domain.Entities;

public class Role : BaseEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public string Name { get; private set; } = string.Empty;
    public bool IsSystemDefault { get; private set; }

    private readonly List<RolePermission> _rolePermissions = new();
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    private Role() { }

    public Role(Guid id, Guid tenantId, string name, bool isSystemDefault = false) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty.", nameof(name));

        TenantId = tenantId;
        Name = name;
        IsSystemDefault = isSystemDefault;
    }

    public void AddPermission(int permissionId)
    {
        if (!_rolePermissions.Any(rp => rp.PermissionId == permissionId))
        {
            _rolePermissions.Add(new RolePermission(Id, permissionId));
        }
    }
}