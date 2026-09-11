using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Application.Firmware.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Firmware.Queries.GetDeviceFirmwareInventoryPaged;

public class GetDeviceFirmwareInventoryPagedQueryHandler : IRequestHandler<GetDeviceFirmwareInventoryPagedQuery, PagedResult<DeviceFirmwareInventoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly IFirmwareVersionComparator _versionComparator;

    public GetDeviceFirmwareInventoryPagedQueryHandler(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext,
        IFirmwareVersionComparator versionComparator)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _versionComparator = versionComparator;
    }

    public async Task<PagedResult<DeviceFirmwareInventoryDto>> Handle(GetDeviceFirmwareInventoryPagedQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;

        var allDevices = await _unitOfWork.Devices.GetAllAsync(cancellationToken);
        var tenantDevices = allDevices.Where(d => d.TenantId == tenantId).ToList();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLower();
            tenantDevices = tenantDevices.Where(d =>
                d.Name.ToLower().Contains(term) ||
                d.IpAddress.ToLower().Contains(term) ||
                (d.Vendor != null && d.Vendor.ToLower().Contains(term)) ||
                (d.Model != null && d.Model.ToLower().Contains(term)) ||
                (d.FirmwareVersion != null && d.FirmwareVersion.ToLower().Contains(term))).ToList();
        }

        var activeBaselines = await _unitOfWork.FirmwareBaselines.GetActiveBaselinesAsync(tenantId, cancellationToken);
        var baselineMap = activeBaselines.ToDictionary(
            b => $"{b.Vendor.ToLower()}:{b.Model.ToLower()}",
            b => b.TargetVersion);

        var inventoryList = new List<DeviceFirmwareInventoryDto>();

        foreach (var device in tenantDevices)
        {
            string? targetVersion = null;
            if (!string.IsNullOrWhiteSpace(device.Vendor) && !string.IsNullOrWhiteSpace(device.Model))
            {
                var key = $"{device.Vendor.ToLower()}:{device.Model.ToLower()}";
                baselineMap.TryGetValue(key, out targetVersion);
            }

            var compliance = _versionComparator.EvaluateCompliance(device.FirmwareVersion, targetVersion);

            if (request.ComplianceStatus.HasValue && compliance != request.ComplianceStatus.Value)
            {
                continue;
            }

            inventoryList.Add(new DeviceFirmwareInventoryDto(
                device.Id,
                device.Name,
                device.IpAddress,
                device.DeviceType,
                device.Vendor,
                device.Model,
                device.FirmwareVersion,
                targetVersion,
                compliance,
                device.LastSeenUtc));
        }

        var totalCount = inventoryList.Count;
        var pagedItems = inventoryList
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedResult<DeviceFirmwareInventoryDto>(pagedItems, totalCount, request.PageNumber, request.PageSize);
    }
}