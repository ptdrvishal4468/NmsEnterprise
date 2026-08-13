namespace Nms.Infrastructure.Snmp.Options;

public sealed class SnmpTrapOptions
{
    public const string SectionName = "SnmpTrap";

    public int Port { get; set; } = 162;
    public int BufferSize { get; set; } = 10000;
    public int BatchSize { get; set; } = 100;
    public int ProcessingIntervalMs { get; set; } = 500;
    public bool Enabled { get; set; } = true;
}