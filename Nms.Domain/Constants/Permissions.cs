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

    public static class Tenants
    {
        public const string View = "Tenants.View";
        public const string Create = "Tenants.Create";
        public const string Update = "Tenants.Update";
        public const string Delete = "Tenants.Delete";
    }

    public static class Discovery
    {
        public const string Scan = "Discovery.Scan";
        public const string View = "Discovery.View";
        public const string Import = "Discovery.Import";
    }

    public static class Interfaces
    {
        public const string View = "Interfaces.View";
        public const string Manage = "Interfaces.Manage";
        public const string Poll = "Interfaces.Poll";
    }

    public static class Health
    {
        public const string View = "Health.View";
        public const string History = "Health.History";
        public const string Evaluate = "Health.Evaluate";
    }

    public static class Alerts
    {
        public const string View = "Alerts.View";
        public const string ManageRules = "Alerts.ManageRules";
        public const string Acknowledge = "Alerts.Acknowledge";
        public const string Suppress = "Alerts.Suppress";
        public const string Resolve = "Alerts.Resolve";
    }

    public static class Notifications
    {
        public const string View = "Notifications.View";
        public const string ManageTemplates = "Notifications.ManageTemplates";
        public const string SendTest = "Notifications.SendTest";
    }

    public static class Events
    {
        public const string View = "Events.View";
        public const string Record = "Events.Record";
        public const string Manage = "Events.Manage";
    }

    public static class Syslog
    {
        public const string View = "Syslog.View";
        public const string Search = "Syslog.Search";
        public const string Manage = "Syslog.Manage";
    }

    public static class SnmpTrap
    {
        public const string View = "SnmpTrap.View";
        public const string Manage = "SnmpTrap.Manage";
        public const string Delete = "SnmpTrap.Delete";
    }

    public static class ConfigurationBackup
    {
        public const string View = "ConfigurationBackup.View";
        public const string Create = "ConfigurationBackup.Create";
        public const string Delete = "ConfigurationBackup.Delete";
        public const string Download = "ConfigurationBackup.Download";
        public const string Schedule = "ConfigurationBackup.Schedule";
        public const string Restore = "ConfigurationBackup.Restore";
    }

    public static class Firmware
    {
        public const string View = "Firmware.View";
        public const string ManageBaselines = "Firmware.ManageBaselines";
        public const string ManagePlans = "Firmware.ManagePlans";
    }

    public static class Topology
    {
        public const string View = "Topology.View";
        public const string Discover = "Topology.Discover";
        public const string Manage = "Topology.Manage";
    }

    public static class Dashboard
    {
        public const string View = "Dashboard.View";
        public const string ViewExecutive = "Dashboard.ViewExecutive";
        public const string ViewTenant = "Dashboard.ViewTenant";
    }

    public static class Reports
    {
        public const string Generate = "Reports.Generate";
        public const string ManageSchedules = "Reports.ManageSchedules";
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
            Telemetry.View, Telemetry.Poll, Telemetry.Export,
            Tenants.View, Tenants.Create, Tenants.Update, Tenants.Delete,
            Discovery.Scan, Discovery.View, Discovery.Import,
            Interfaces.View, Interfaces.Manage, Interfaces.Poll,
            Health.View, Health.History, Health.Evaluate,
            Alerts.View, Alerts.ManageRules, Alerts.Acknowledge, Alerts.Suppress, Alerts.Resolve,
            Notifications.View, Notifications.ManageTemplates, Notifications.SendTest,
            Events.View, Events.Record, Events.Manage,
            Syslog.View, Syslog.Search, Syslog.Manage,
            SnmpTrap.View, SnmpTrap.Manage, SnmpTrap.Delete,
            ConfigurationBackup.View, ConfigurationBackup.Create, ConfigurationBackup.Delete, ConfigurationBackup.Download, ConfigurationBackup.Schedule, ConfigurationBackup.Restore,
            Firmware.View, Firmware.ManageBaselines, Firmware.ManagePlans,
            Topology.View, Topology.Discover, Topology.Manage,
            Dashboard.View, Dashboard.ViewExecutive, Dashboard.ViewTenant,
            Reports.Generate, Reports.ManageSchedules
        };
    }
}