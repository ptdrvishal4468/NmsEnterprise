namespace Nms.Domain.Entities;

public class RolePermission
{
    public Guid RoleId { get; private set; }
    public int PermissionId { get; private set; }

    private RolePermission() { }

    public RolePermission(Guid roleId, int permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }
}