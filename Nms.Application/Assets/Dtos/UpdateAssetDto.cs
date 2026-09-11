using Nms.Domain.Enums;

namespace Nms.Application.Assets.Dtos;

public class UpdateAssetDto
{
    public string Name { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public string? Vendor { get; set; }
    public string? Model { get; set; }
    public string? Category { get; set; }

    public string? SiteOrLocation { get; set; }
    public string? RackIdentifier { get; set; }
    public string? RackUnitPosition { get; set; }

    public string? Department { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public Guid? DeviceId { get; set; }

    public string? WarrantyProvider { get; set; }
    public DateTime? WarrantyStartDateUtc { get; set; }
    public DateTime? WarrantyEndDateUtc { get; set; }
    public WarrantyStatus WarrantyStatus { get; set; }
    public string? WarrantyContractNumber { get; set; }

    public DateTime? PurchaseDateUtc { get; set; }
    public string? PurchaseOrderNumber { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string? Currency { get; set; }
}