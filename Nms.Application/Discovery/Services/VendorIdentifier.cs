namespace Nms.Application.Discovery.Services;

public static class VendorIdentifier
{
    private static readonly Dictionary<string, string> PenVendorMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { ".1.3.6.1.4.1.9", "Cisco Systems" },
        { ".1.3.6.1.4.1.11", "HP / Aruba" },
        { ".1.3.6.1.4.1.2636", "Juniper Networks" },
        { ".1.3.6.1.4.1.8072", "Net-SNMP" },
        { ".1.3.6.1.4.1.311", "Microsoft Corporation" },
        { ".1.3.6.1.4.1.14823", "Aruba Networks" },
        { ".1.3.6.1.4.1.1588", "Brocade" },
        { ".1.3.6.1.4.1.2011", "Huawei" },
        { ".1.3.6.1.4.1.12356", "Fortinet" },
        { ".1.3.6.1.4.1.25053", "Ruckus Wireless" },
        { ".1.3.6.1.4.1.14988", "MikroTik" },
        { ".1.3.6.1.4.1.41112", "Ubiquiti Networks" }
    };

    public static string IdentifyVendor(string? sysObjectId, string? sysDescr)
    {
        if (!string.IsNullOrWhiteSpace(sysObjectId))
        {
            var normalizedOid = sysObjectId.StartsWith('.') ? sysObjectId : "." + sysObjectId;

            foreach (var (pen, vendor) in PenVendorMap)
            {
                if (normalizedOid.StartsWith(pen, StringComparison.OrdinalIgnoreCase))
                {
                    return vendor;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(sysDescr))
        {
            var descr = sysDescr.ToLowerInvariant();
            if (descr.Contains("cisco")) return "Cisco Systems";
            if (descr.Contains("juniper")) return "Juniper Networks";
            if (descr.Contains("aruba") || descr.Contains("procurve")) return "HP / Aruba";
            if (descr.Contains("fortinet") || descr.Contains("fortigate")) return "Fortinet";
            if (descr.Contains("mikrotik") || descr.Contains("routeros")) return "MikroTik";
            if (descr.Contains("ubiquiti") || descr.Contains("unifi")) return "Ubiquiti Networks";
            if (descr.Contains("huawei")) return "Huawei";
            if (descr.Contains("linux")) return "Linux / Open Source";
            if (descr.Contains("windows")) return "Microsoft Corporation";
        }

        return "Unknown";
    }
}