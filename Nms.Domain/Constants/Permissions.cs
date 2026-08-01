namespace Nms.Domain.Constants;

public static class Permissions
{
    public static class Users
    {
        public const string View = "Users.View";
        public const string Create = "Users.Create";
        public const string Update = "Users.Update";
        public const string Delete = "Users.Delete";
        public const string ManageRoles = "Users.ManageRoles";
    }

    public static class Roles
    {
        public const string View = "Roles.View";
        public const string Create = "Roles.Create";
        public const string Update = "Roles.Update";
        public const string Delete = "Roles.Delete";
        public const string AssignPermissions = "Roles.AssignPermissions";
    }

    public static class Devices
    {
        public const string View = "Devices.View";
        public const string Create = "Devices.Create";
        public const string Update = "Devices.Update";
        public const string Delete = "Devices.Delete";
        public const string Control = "Devices.Control";
    }

    public static class Telemetry
    {
        public const string View = "Telemetry.View";
        public const string Poll = "Telemetry.Poll";
        public const string Export = "Telemetry.Export";
    }

    /// <summary>
    /// Helper method to retrieve all predefined permissions for seeding and validation.
    /// </summary>
    public static IReadOnlyList<string> GetAllPermissions()
    {
        return new List<string>
        {
            Users.View, Users.Create, Users.Update, Users.Delete, Users.ManageRoles,
            Roles.View, Roles.Create, Roles.Update, Roles.Delete, Roles.AssignPermissions,
            Devices.View, Devices.Create, Devices.Update, Devices.Delete, Devices.Control,
            Telemetry.View, Telemetry.Poll, Telemetry.Export
        };
    }
}