using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Devices.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Devices.Commands.UpdateDevice;

public class UpdateDeviceCommandHandler : IRequestHandler<UpdateDeviceCommand, DeviceDto>
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDeviceCommandHandler(
        IDeviceRepository deviceRepository,
        ITenantContext tenantContext,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
        _tenantContext = tenantContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeviceDto> Handle(UpdateDeviceCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.IsResolved || _tenantContext.TenantId == Guid.Empty)
        {
            throw new InvalidOperationException("A valid tenant context is required to update a device.");
        }

        var device = await _deviceRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Device with ID '{request.Id}' was not found.");

        // Check if IP address is being changed to one that belongs to another device in this tenant
        var ipExistsOnAnotherDevice = await _deviceRepository.ExistsByIpAddressExcludingIdAsync(
            request.IpAddress.Trim(),
            request.Id,
            cancellationToken);

        if (ipExistsOnAnotherDevice)
        {
            throw new InvalidOperationException($"Another device with IP address '{request.IpAddress.Trim()}' already exists for this tenant.");
        }

        device.UpdateInventory(
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

        _deviceRepository.Update(device);
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