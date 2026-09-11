using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Devices.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Discovery.Commands.ImportDiscoveredDevice;

public class ImportDiscoveredDeviceCommandHandler : IRequestHandler<ImportDiscoveredDeviceCommand, DeviceDto>
{
    private readonly ITenantContext _tenantContext;
    private readonly IDiscoveryJobRepository _jobRepository;
    private readonly IDeviceRepository _deviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ImportDiscoveredDeviceCommandHandler(
        ITenantContext tenantContext,
        IDiscoveryJobRepository jobRepository,
        IDeviceRepository deviceRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantContext = tenantContext;
        _jobRepository = jobRepository;
        _deviceRepository = deviceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeviceDto> Handle(ImportDiscoveredDeviceCommand request, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetWithCandidatesAsync(request.JobId, cancellationToken);
        if (job == null)
            throw new KeyNotFoundException($"Discovery job '{request.JobId}' was not found.");

        var candidate = job.Candidates.FirstOrDefault(c => c.Id == request.CandidateId);
        if (candidate == null)
            throw new KeyNotFoundException($"Candidate '{request.CandidateId}' was not found in job '{request.JobId}'.");

        if (candidate.IsImported)
            throw new InvalidOperationException($"Candidate '{request.CandidateId}' has already been imported.");

        var device = new Device(
            id: Guid.NewGuid(),
            tenantId: _tenantContext.TenantId,
            name: request.DeviceName,
            ipAddress: candidate.IpAddress,
            deviceType: candidate.FingerprintedType,
            snmpPort: job.SnmpPort,
            hostname: candidate.SysName,
            vendor: candidate.IdentifiedVendor,
            model: null,
            serialNumber: null,
            firmwareVersion: candidate.SysDescr,
            macAddress: candidate.MacAddress);

        await _deviceRepository.AddAsync(device, cancellationToken);
        candidate.MarkImported(device.Id);

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
            LastSeenUtc = device.LastSeenUtc
        };
    }
}