namespace Nms.Application.ThreatDetection.Dtos;

public class ThreatSummaryDto
{
    public int TotalActiveThreats { get; set; }
    public int CriticalThreats { get; set; }
    public int HighThreats { get; set; }
    public int MediumThreats { get; set; }
    public int LowThreats { get; set; }
    public int DevicesWithConfigDrift { get; set; }
    public int FailedLoginThreats { get; set; }
    public int PortScanThreats { get; set; }
    public int UnauthorizedAccessThreats { get; set; }
}