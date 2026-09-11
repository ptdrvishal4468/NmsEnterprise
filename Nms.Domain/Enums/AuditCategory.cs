namespace Nms.Domain.Enums;

/// <summary>
/// Defines the operational categories for audit logging and compliance segmentation.
/// </summary>
public enum AuditCategory
{
    System = 0,
    Authentication = 1,
    UserManagement = 2,
    RoleManagement = 3,
    DeviceManagement = 4,
    Configuration = 5,
    Alerts = 6,
    Reports = 7,
    Security = 8
}