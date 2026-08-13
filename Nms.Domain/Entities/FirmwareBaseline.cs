using Nms.Domain.Common;

namespace Nms.Domain.Entities;

public class FirmwareBaseline : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public string Vendor { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public string TargetVersion { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public string? Notes { get; private set; }

    // EF Core private constructor
    private FirmwareBaseline() { }

    public FirmwareBaseline(
        Guid id,
        Guid tenantId,
        string vendor,
        string model,
        string targetVersion,
        string? notes = null,
        bool isActive = true) : base(id)
    {
        if (string.IsNullOrWhiteSpace(vendor))
            throw new ArgumentException("Vendor is required.", nameof(vendor));

        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model is required.", nameof(model));

        if (string.IsNullOrWhiteSpace(targetVersion))
            throw new ArgumentException("Target version is required.", nameof(targetVersion));

        TenantId = tenantId;
        Vendor = vendor.Trim();
        Model = model.Trim();
        TargetVersion = targetVersion.Trim();
        Notes = notes;
        IsActive = isActive;
    }

    public void UpdateBaseline(string targetVersion, string? notes, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(targetVersion))
            throw new ArgumentException("Target version is required.", nameof(targetVersion));

        TargetVersion = targetVersion.Trim();
        Notes = notes;
        IsActive = isActive;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}