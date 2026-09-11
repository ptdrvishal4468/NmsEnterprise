using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Devices.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Devices.Commands.CreateDevice;

public class CreateDeviceCommandHandler : IRequestHandler<CreateDeviceCommand, DeviceDto>
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDeviceCommandHandler(
        IDeviceRepository deviceRepository,
        ITenantContext tenantContext,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
        _tenantContext = tenantContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeviceDto> Handle(CreateDeviceCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.IsResolved || _tenantContext.TenantId == Guid.Empty)
        {
            throw new InvalidOperationException("A valid tenant context is required to create a device.");
        }

        var tenantId = _tenantContext.TenantId;

        // Enforce tenant-scoped IP address uniqueness
        var ipExists = await _deviceRepository.ExistsByIpAddressAsync(request.IpAddress.Trim(), cancellationToken);
        if (ipExists)
        {
            throw new InvalidOperationException($"A device with IP address '{request.IpAddress.Trim()}' already exists for this tenant.");
        }

        var device = new Device(
            id: Guid.NewGuid(),
            tenantId: tenantId,
            name: request.Name.Trim(),
            ipAddress: request.IpAddress.Trim(),
            deviceType: request.DeviceType,
            snmpPort: request.SnmpPort,
            hostname: request.Hostname?.Trim(),
            vendor: request.Vendor?.Trim(),
            model: request.Model?.Trim(),
            serialNumber: request.SerialNumber?.Trim(),
            firmwareVersion: request.FirmwareVersion?.Trim(),
            macAddress: request.MacAddress?.Trim(),
            site: request.Site?.Trim(),
            location: request.Location?.Trim()
        );

        await _deviceRepository.AddAsync(device, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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