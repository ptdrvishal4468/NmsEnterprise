using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Vulnerabilities.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Vulnerabilities.Commands.GenerateUpgradeRecommendations;

public class GenerateUpgradeRecommendationsCommandHandler : IRequestHandler<GenerateUpgradeRecommendationsCommand, IReadOnlyList<FirmwareUpgradeRecommendationDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly IFirmwareUpgradeRecommendationEngine _recommendationEngine;

    public GenerateUpgradeRecommendationsCommandHandler(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext,
        IFirmwareUpgradeRecommendationEngine recommendationEngine)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _recommendationEngine = recommendationEngine;
    }

    public async Task<IReadOnlyList<FirmwareUpgradeRecommendationDto>> Handle(GenerateUpgradeRecommendationsCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var devices = (await _unitOfWork.Devices.GetAllAsync(cancellationToken)).Where(d => d.TenantId == tenantId).ToList();

        var recommendations = new List<FirmwareUpgradeRecommendationDto>();

        foreach (var device in devices)
        {
            var matches = await _unitOfWork.DeviceVulnerabilityMatches.GetByDeviceIdAsync(tenantId, device.Id, cancellationToken);
            var activeMatches = matches.Where(m => m.MatchStatus == VulnerabilityStatus.Active).ToList();

            if (activeMatches.Count > 0)
            {
                var rec = await _recommendationEngine.GenerateRecommendationForDeviceAsync(tenantId, device, activeMatches, cancellationToken);
                if (rec != null)
                {
                    recommendations.Add(new FirmwareUpgradeRecommendationDto(
                        rec.Id,
                        rec.TenantId,
                        rec.DeviceId,
                        device.Name,
                        rec.CurrentVersion,
                        rec.RecommendedVersion,
                        rec.Priority,
                        rec.Reasoning,
                        rec.AssociatedVulnerabilityCount,
                        rec.CriticalVulnerabilityCount,
                        rec.IsApplied,
                        rec.CreatedAtUtc));
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return recommendations;
    }
}