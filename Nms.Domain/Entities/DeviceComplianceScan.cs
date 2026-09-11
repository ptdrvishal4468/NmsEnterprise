using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class DeviceComplianceScan : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid DeviceId { get; private set; }
    public Device? Device { get; private set; }
    public DateTime ScannedAtUtc { get; private set; } = DateTime.UtcNow;
    public ComplianceStatus OverallStatus { get; private set; } = ComplianceStatus.UnableToEvaluate;
    public int PassedChecks { get; private set; }
    public int FailedChecks { get; private set; }
    public int WarningChecks { get; private set; }
    public int NotApplicableChecks { get; private set; }
    public int TotalChecks { get; private set; }
    public string? EvaluationNotes { get; private set; }

    // Granular check results collection
    public ICollection<DeviceComplianceResult> Results { get; private set; } = new List<DeviceComplianceResult>();

    // EF Core materialization constructor
    private DeviceComplianceScan() { }

    public DeviceComplianceScan(
        Guid id,
        Guid tenantId,
        Guid deviceId,
        DateTime scannedAtUtc,
        string? evaluationNotes = null) : base(id)
    {
        if (deviceId == Guid.Empty)
            throw new ArgumentException("Valid DeviceId is required.", nameof(deviceId));

        TenantId = tenantId;
        DeviceId = deviceId;
        ScannedAtUtc = scannedAtUtc;
        EvaluationNotes = evaluationNotes;
        OverallStatus = ComplianceStatus.UnableToEvaluate;
    }

    public void AddResult(DeviceComplianceResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        Results.Add(result);
        RecalculateTotals();
    }

    public void RecalculateTotals()
    {
        PassedChecks = Results.Count(r => r.Status == ComplianceStatus.Compliant);
        FailedChecks = Results.Count(r => r.Status == ComplianceStatus.NonCompliant);
        WarningChecks = Results.Count(r => r.Status == ComplianceStatus.Warning);
        NotApplicableChecks = Results.Count(r => r.Status == ComplianceStatus.NotApplicable);
        TotalChecks = Results.Count;

        if (TotalChecks == 0)
        {
            OverallStatus = ComplianceStatus.UnableToEvaluate;
        }
        else if (FailedChecks > 0)
        {
            OverallStatus = ComplianceStatus.NonCompliant;
        }
        else if (WarningChecks > 0)
        {
            OverallStatus = ComplianceStatus.Warning;
        }
        else if (PassedChecks > 0)
        {
            OverallStatus = ComplianceStatus.Compliant;
        }
        else
        {
            OverallStatus = ComplianceStatus.NotApplicable;
        }
    }
}