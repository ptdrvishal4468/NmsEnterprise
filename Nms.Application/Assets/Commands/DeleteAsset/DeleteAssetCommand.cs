using MediatR;

namespace Nms.Application.Assets.Commands.DeleteAsset;

public record DeleteAssetCommand(Guid Id) : IRequest<bool>;