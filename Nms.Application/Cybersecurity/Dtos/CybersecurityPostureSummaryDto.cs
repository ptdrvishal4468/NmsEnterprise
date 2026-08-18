namespace Nms.Application.Cybersecurity.Dtos;

public class CybersecurityPostureSummaryDto
{
    public int TotalScannedDevices { get; set; }
    public int CompliantDevices { get; set; }
    public int NonCompliantDevices { get; set; }
    public int WarningDevices { get; set; }
    public int UnableToEvaluateDevices { get; set; }
    public double CompliancePercentage { get; set; }
    public DateTime GeneratedAtUtc { get; set; } = DateTime.UtcNow;
}