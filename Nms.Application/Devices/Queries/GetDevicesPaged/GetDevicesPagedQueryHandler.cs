using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Devices.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Devices.Queries.GetDevicesPaged;

public class GetDevicesPagedQueryHandler : IRequestHandler<GetDevicesPagedQuery, PagedResult<DeviceDto>>
{
    private readonly IDeviceRepository _deviceRepository;

    public GetDevicesPagedQueryHandler(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<PagedResult<DeviceDto>> Handle(GetDevicesPagedQuery request, CancellationToken cancellationToken)
    {
        var pageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
        var pageSize = request.PageSize < 1 ? 10 : (request.PageSize > 100 ? 100 : request.PageSize);

        var (items, totalCount) = await _deviceRepository.GetPagedAsync(
            pageIndex,
            pageSize,
            request.SearchTerm,
            request.DeviceType,
            request.Status,
            cancellationToken);

        var dtos = items.Select(d => new DeviceDto
        {
            Id = d.Id,
            TenantId = d.TenantId,
            Name = d.Name,
            IpAddress = d.IpAddress,
            Hostname = d.Hostname,
            Vendor = d.Vendor,
            Model = d.Model,
            SerialNumber = d.SerialNumber,
            FirmwareVersion = d.FirmwareVersion,
            MacAddress = d.MacAddress,
            Site = d.Site,
            Location = d.Location,
            DeviceType = d.DeviceType,
            SnmpPort = d.SnmpPort,
            Status = d.Status,
            LastSeenUtc = d.LastSeenUtc,
            CreatedAtUtc = d.CreatedAtUtc,
            LastModifiedAtUtc = d.LastModifiedAtUtc
        }).ToList();

        return new PagedResult<DeviceDto>(dtos, totalCount, pageIndex, pageSize);
    }
}