using Nms.Domain.Enums;

namespace Nms.Application.Assets.Dtos;

public class ChangeAssetLifecycleDto
{
    public AssetLifecycleState LifecycleState { get; set; }
    public string? Notes { get; set; }
}