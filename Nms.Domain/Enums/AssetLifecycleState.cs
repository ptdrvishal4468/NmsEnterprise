namespace Nms.Domain.Enums;

/// <summary>
/// Represents the physical and operational lifecycle states of an asset.
/// </summary>
public enum AssetLifecycleState
{
    Planned = 0,
    Ordered = 1,
    Received = 2,
    InStock = 3,
    InService = 4,
    Maintenance = 5,
    Retired = 6,
    Disposed = 7
}