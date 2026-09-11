using MediatR;
using Nms.Application.Assets.Dtos;

namespace Nms.Application.Assets.Queries.GetAssetById;

public record GetAssetByIdQuery(Guid Id) : IRequest<AssetDto?>;