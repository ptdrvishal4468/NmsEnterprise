using MediatR;
using Nms.Application.Vulnerabilities.Dtos;

namespace Nms.Application.Vulnerabilities.Commands.GenerateUpgradeRecommendations;

public record GenerateUpgradeRecommendationsCommand : IRequest<IReadOnlyList<FirmwareUpgradeRecommendationDto>>;