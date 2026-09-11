using Nms.Domain.Enums;

namespace Nms.Application.Discovery.Services;

public static class DeviceFingerprinter
{
    public static DeviceType FingerprintDevice(string? sysObjectId, string? sysDescr)
    {
        if (string.IsNullOrWhiteSpace(sysDescr) && string.IsNullOrWhiteSpace(sysObjectId))
        {
            return DeviceType.Unknown;
        }

        var descr = (sysDescr ?? string.Empty).ToLowerInvariant();

        if (descr.Contains("router") || descr.Contains("routeros") || descr.Contains("ios-xe") || descr.Contains("vyos"))
            return DeviceType.Router;

        if (descr.Contains("switch") || descr.Contains("catalyst") || descr.Contains("procurve") || descr.Contains("nexus") || descr.Contains("c3560") || descr.Contains("c3850") || descr.Contains("c9200") || descr.Contains("c9300") || descr.Contains("ios software"))
            return DeviceType.Switch;

        if (descr.Contains("firewall") || descr.Contains("fortigate") || descr.Contains("asa") || descr.Contains("palo alto"))
            return DeviceType.Firewall;

        if (descr.Contains("wireless controller") || descr.Contains("wlc"))
            return DeviceType.WirelessController;

        if (descr.Contains("access point") || descr.Contains(" ap ") || descr.Contains("unifi ap"))
            return DeviceType.AccessPoint;

        if (descr.Contains("linux") || descr.Contains("ubuntu") || descr.Contains("debian") || descr.Contains("centos") || descr.Contains("red hat"))
            return DeviceType.LinuxServer;

        if (descr.Contains("windows"))
            return DeviceType.WindowsServer;

        if (descr.Contains("printer") || descr.Contains("jetdirect"))
            return DeviceType.Printer;

        return DeviceType.Unknown;
    }
}