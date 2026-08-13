using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Firmware.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Firmware.Queries.GetFirmwareComplianceSummary;

public class GetFirmwareComplianceSummaryQueryHandler : IRequestHandler<GetFirmwareComplianceSummaryQuery, FirmwareComplianceSummaryDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly IFirmwareVersionComparator _versionComparator;

    public GetFirmwareComplianceSummaryQueryHandler(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext,
        IFirmwareVersionComparator versionComparator)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _versionComparator = versionComparator;
    }

    public async Task<FirmwareComplianceSummaryDto> Handle(GetFirmwareComplianceSummaryQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;

        var allDevices = await _unitOfWork.Devices.GetAllAsync(cancellationToken);
        var tenantDevices = allDevices.Where(d => d.TenantId == tenantId).ToList();

        var activeBaselines = await _unitOfWork.FirmwareBaselines.GetActiveBaselinesAsync(tenantId, cancellationToken);
        var baselineMap = activeBaselines.ToDictionary(
            b => $"{b.Vendor.ToLower()}:{b.Model.ToLower()}",
            b => b.TargetVersion);

        int total = tenantDevices.Count;
        int compliant = 0;
        int nonCompliant = 0;
        int unknown = 0;

        foreach (var device in tenantDevices)
        {
            string? targetVersion = null;
            if (!string.IsNullOrWhiteSpace(device.Vendor) && !string.IsNullOrWhiteSpace(device.Model))
            {
                var key = $"{device.Vendor.ToLower()}:{device.Model.ToLower()}";
                baselineMap.TryGetValue(key, out targetVersion);
            }

            var status = _versionComparator.EvaluateCompliance(device.FirmwareVersion, targetVersion);
            switch (status)
            {
                case FirmwareComplianceStatus.Compliant:
                    compliant++;
                    break;
                case FirmwareComplianceStatus.NonCompliant:
                    nonCompliant++;
                    break;
                default:
                    unknown++;
                    break;
            }
        }

        double percentage = total > 0 ? Math.Round((double)compliant / total * 100, 2) : 0.0;

        return new FirmwareComplianceSummaryDto(total, compliant, nonCompliant, unknown, percentage);
    }
}