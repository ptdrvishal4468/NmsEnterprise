namespace Nms.Infrastructure.Syslog.Options;

public class SyslogOptions
{
    public const string SectionName = "Syslog";

    public int Port { get; set; } = 514;
    public int BufferSize { get; set; } = 10000;
    public int BatchSize { get; set; } = 100;
    public int ProcessingIntervalMs { get; set; } = 500;
    public bool Enabled { get; set; } = true;
}