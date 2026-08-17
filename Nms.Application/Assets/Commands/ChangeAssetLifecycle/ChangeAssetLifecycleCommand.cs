using MediatR;
using Nms.Application.Assets.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.Assets.Commands.ChangeAssetLifecycle;

public record ChangeAssetLifecycleCommand(
    Guid Id,
    AssetLifecycleState LifecycleState,
    string? Notes = null) : IRequest<AssetDto?>;