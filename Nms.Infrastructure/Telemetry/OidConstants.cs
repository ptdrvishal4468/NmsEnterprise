namespace Nms.Infrastructure.Telemetry;

public static class OidConstants
{
    // Standard MIB-II System Group
    public const string SystemUptime = "1.3.6.1.2.1.1.3.0";
    public const string SystemName = "1.3.6.1.2.1.1.5.0";
    public const string SystemDescription = "1.3.6.1.2.1.1.1.0";

    // Host Resources MIB (Processor / Memory / Storage)
    public const string HostMemoryAllocationUnits = "1.3.6.1.2.1.25.2.3.1.4";
    public const string HostMemorySize = "1.3.6.1.2.1.25.2.3.1.5";
    public const string HostMemoryUsed = "1.3.6.1.2.1.25.2.3.1.6";
    public const string DiskUtilization = "1.3.6.1.2.1.25.2.3.1.6.1";

    // Cisco Specific OIDs
    public const string CiscoCpu5Min = "1.3.6.1.4.1.9.9.109.1.1.1.1.5.1";
    public const string CiscoMemoryPoolUsed = "1.3.6.1.4.1.9.9.48.1.1.1.5.1";
    public const string CiscoMemoryPoolFree = "1.3.6.1.4.1.9.9.48.1.1.1.6.1";
    public const string CiscoEnvMonTemperature = "1.3.6.1.4.1.9.9.13.1.3.1.3.1";
    public const string CiscoEnvMonFanStatus = "1.3.6.1.4.1.9.9.13.1.4.1.3.1";
    public const string CiscoEnvMonSupplyStatus = "1.3.6.1.4.1.9.9.13.1.5.1.3.1";

    // Standard Interfaces MIB
    public const string IfInOctets = "1.3.6.1.2.1.2.2.1.10";
    public const string IfOutOctets = "1.3.6.1.2.1.2.2.1.16";
    public const string InterfaceUtilization = "1.3.6.1.2.1.2.2.1.10.1";
}