using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

/// <summary>
/// Physical infrastructure asset aggregate root encapsulating lifecycle, warranty, purchase, rack location, and ownership.
/// </summary>
public class Asset : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public string AssetTag { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? SerialNumber { get; private set; }
    public string? Vendor { get; private set; }
    public string? Model { get; private set; }
    public string? Category { get; private set; }

    // Lifecycle
    public AssetLifecycleState LifecycleState { get; private set; } = AssetLifecycleState.Planned;
    public string? LifecycleNotes { get; private set; }
    public DateTime? LifecycleChangedAtUtc { get; private set; }

    // Warranty
    public string? WarrantyProvider { get; private set; }
    public DateTime? WarrantyStartDateUtc { get; private set; }
    public DateTime? WarrantyEndDateUtc { get; private set; }
    public WarrantyStatus WarrantyStatus { get; private set; } = WarrantyStatus.NotApplicable;
    public string? WarrantyContractNumber { get; private set; }

    // Purchase Details
    public DateTime? PurchaseDateUtc { get; private set; }
    public string? PurchaseOrderNumber { get; private set; }
    public decimal? PurchasePrice { get; private set; }
    public string? Currency { get; private set; }

    // Physical Rack Location
    public string? SiteOrLocation { get; private set; }
    public string? RackIdentifier { get; private set; }
    public string? RackUnitPosition { get; private set; }

    // Ownership & Association
    public string? Department { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public User? AssignedToUser { get; private set; }
    public Guid? DeviceId { get; private set; }
    public Device? Device { get; private set; }

    // EF Core materialization constructor
    private Asset() { }

    public Asset(
        Guid id,
        Guid tenantId,
        string assetTag,
        string name,
        AssetLifecycleState lifecycleState = AssetLifecycleState.Planned,
        string? serialNumber = null,
        string? vendor = null,
        string? model = null,
        string? category = null,
        string? siteOrLocation = null,
        string? rackIdentifier = null,
        string? rackUnitPosition = null,
        string? department = null,
        Guid? assignedToUserId = null,
        Guid? deviceId = null,
        string? warrantyProvider = null,
        DateTime? warrantyStartDateUtc = null,
        DateTime? warrantyEndDateUtc = null,
        WarrantyStatus warrantyStatus = WarrantyStatus.NotApplicable,
        string? warrantyContractNumber = null,
        DateTime? purchaseDateUtc = null,
        string? purchaseOrderNumber = null,
        decimal? purchasePrice = null,
        string? currency = "USD") : base(id)
    {
        if (string.IsNullOrWhiteSpace(assetTag))
            throw new ArgumentException("Asset tag is required.", nameof(assetTag));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Asset name is required.", nameof(name));

        if (purchasePrice.HasValue && purchasePrice.Value < 0)
            throw new ArgumentException("Purchase price cannot be negative.", nameof(purchasePrice));

        TenantId = tenantId;
        AssetTag = assetTag.Trim();
        Name = name.Trim();
        LifecycleState = lifecycleState;
        LifecycleChangedAtUtc = DateTime.UtcNow;
        SerialNumber = serialNumber?.Trim();
        Vendor = vendor?.Trim();
        Model = model?.Trim();
        Category = category?.Trim();

        SiteOrLocation = siteOrLocation?.Trim();
        RackIdentifier = rackIdentifier?.Trim();
        RackUnitPosition = rackUnitPosition?.Trim();

        Department = department?.Trim();
        AssignedToUserId = assignedToUserId;
        DeviceId = deviceId;

        WarrantyProvider = warrantyProvider?.Trim();
        WarrantyStartDateUtc = warrantyStartDateUtc;
        WarrantyEndDateUtc = warrantyEndDateUtc;
        WarrantyStatus = warrantyStatus;
        WarrantyContractNumber = warrantyContractNumber?.Trim();

        PurchaseDateUtc = purchaseDateUtc;
        PurchaseOrderNumber = purchaseOrderNumber?.Trim();
        PurchasePrice = purchasePrice;
        Currency = string.IsNullOrWhiteSpace(currency) ? "USD" : currency.Trim().ToUpperInvariant();
    }

    public void UpdateDetails(
        string name,
        string? serialNumber,
        string? vendor,
        string? model,
        string? category,
        string? siteOrLocation,
        string? rackIdentifier,
        string? rackUnitPosition,
        string? department,
        Guid? assignedToUserId,
        Guid? deviceId,
        string? warrantyProvider,
        DateTime? warrantyStartDateUtc,
        DateTime? warrantyEndDateUtc,
        WarrantyStatus warrantyStatus,
        string? warrantyContractNumber,
        DateTime? purchaseDateUtc,
        string? purchaseOrderNumber,
        decimal? purchasePrice,
        string? currency)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Asset name is required.", nameof(name));

        if (purchasePrice.HasValue && purchasePrice.Value < 0)
            throw new ArgumentException("Purchase price cannot be negative.", nameof(purchasePrice));

        Name = name.Trim();
        SerialNumber = serialNumber?.Trim();
        Vendor = vendor?.Trim();
        Model = model?.Trim();
        Category = category?.Trim();

        SiteOrLocation = siteOrLocation?.Trim();
        RackIdentifier = rackIdentifier?.Trim();
        RackUnitPosition = rackUnitPosition?.Trim();

        Department = department?.Trim();
        AssignedToUserId = assignedToUserId;
        DeviceId = deviceId;

        WarrantyProvider = warrantyProvider?.Trim();
        WarrantyStartDateUtc = warrantyStartDateUtc;
        WarrantyEndDateUtc = warrantyEndDateUtc;
        WarrantyStatus = warrantyStatus;
        WarrantyContractNumber = warrantyContractNumber?.Trim();

        PurchaseDateUtc = purchaseDateUtc;
        PurchaseOrderNumber = purchaseOrderNumber?.Trim();
        PurchasePrice = purchasePrice;
        Currency = string.IsNullOrWhiteSpace(currency) ? "USD" : currency.Trim().ToUpperInvariant();
    }

    public void ChangeLifecycleState(AssetLifecycleState newState, string? notes = null)
    {
        LifecycleState = newState;
        LifecycleNotes = notes?.Trim();
        LifecycleChangedAtUtc = DateTime.UtcNow;
    }

    public void UpdateWarrantyStatus(WarrantyStatus status)
    {
        WarrantyStatus = status;
    }

    public void AssignToDevice(Guid? deviceId)
    {
        DeviceId = deviceId;
    }

    public void AssignToUser(Guid? userId, string? department = null)
    {
        AssignedToUserId = userId;
        if (!string.IsNullOrWhiteSpace(department))
        {
            Department = department.Trim();
        }
    }

    public void UpdateRackLocation(string? siteOrLocation, string? rackIdentifier, string? rackUnitPosition)
    {
        SiteOrLocation = siteOrLocation?.Trim();
        RackIdentifier = rackIdentifier?.Trim();
        RackUnitPosition = rackUnitPosition?.Trim();
    }
}