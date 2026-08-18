using Nms.Domain.Enums;

namespace Nms.Application.Cybersecurity.Dtos;

public class DeviceComplianceScanDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public string? DeviceIpAddress { get; set; }
    public DateTime ScannedAtUtc { get; set; }
    public ComplianceStatus OverallStatus { get; set; }
    public int PassedChecks { get; set; }
    public int FailedChecks { get; set; }
    public int WarningChecks { get; set; }
    public int NotApplicableChecks { get; set; }
    public int TotalChecks { get; set; }
    public string? EvaluationNotes { get; set; }
    public IReadOnlyList<DeviceComplianceResultDto> Results { get; set; } = new List<DeviceComplianceResultDto>();
}