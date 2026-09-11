using MediatR;
using Nms.Application.Devices.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Devices.Queries.GetDeviceById;

public class GetDeviceByIdQueryHandler : IRequestHandler<GetDeviceByIdQuery, DeviceDto?>
{
    private readonly IDeviceRepository _deviceRepository;

    public GetDeviceByIdQueryHandler(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<DeviceDto?> Handle(GetDeviceByIdQuery request, CancellationToken cancellationToken)
    {
        var device = await _deviceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (device == null)
        {
            return null;
        }

        return new DeviceDto
        {
            Id = device.Id,
            TenantId = device.TenantId,
            Name = device.Name,
            IpAddress = device.IpAddress,
            Hostname = device.Hostname,
            Vendor = device.Vendor,
            Model = device.Model,
            SerialNumber = device.SerialNumber,
            FirmwareVersion = device.FirmwareVersion,
            MacAddress = device.MacAddress,
            Site = device.Site,
            Location = device.Location,
            DeviceType = device.DeviceType,
            SnmpPort = device.SnmpPort,
            Status = device.Status,
            LastSeenUtc = device.LastSeenUtc,
            CreatedAtUtc = device.CreatedAtUtc,
            LastModifiedAtUtc = device.LastModifiedAtUtc
        };
    }
}