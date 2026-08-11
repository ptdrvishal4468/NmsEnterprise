using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Interfaces.Dtos;
using Nms.Application.Interfaces.Services;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Interfaces.Commands.PollDeviceInterfaces;

public class PollDeviceInterfacesCommandHandler : IRequestHandler<PollDeviceInterfacesCommand, IReadOnlyList<NetworkInterfaceDto>>
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly INetworkInterfaceRepository _interfaceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PollDeviceInterfacesCommandHandler(
        IDeviceRepository deviceRepository,
        INetworkInterfaceRepository interfaceRepository,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
        _interfaceRepository = interfaceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<NetworkInterfaceDto>> Handle(PollDeviceInterfacesCommand request, CancellationToken cancellationToken)
    {
        var device = await _deviceRepository.GetByIdAsync(request.DeviceId, cancellationToken)
            ?? throw new KeyNotFoundException($"Device with ID {request.DeviceId} was not found.");

        var existingInterfaces = await _interfaceRepository.GetByDeviceIdAsync(device.Id, cancellationToken);
        var nowUtc = DateTime.UtcNow;

        // Default initial port topology baseline if first poll
        if (existingInterfaces.Count == 0)
        {
            var defaultInterfaces = new List<NetworkInterface>
            {
                new(Guid.NewGuid(), device.TenantId, device.Id, 1, "eth0", "Primary WAN Interface", "Ethernet", "00:1A:2B:3C:4D:5E", 1000000000, InterfaceAdminStatus.Up, InterfaceOperStatus.Up, InterfaceDuplex.Full),
                new(Guid.NewGuid(), device.TenantId, device.Id, 2, "eth1", "Internal LAN Interface", "Ethernet", "00:1A:2B:3C:4D:5F", 1000000000, InterfaceAdminStatus.Up, InterfaceOperStatus.Up, InterfaceDuplex.Full)
            };

            foreach (var iface in defaultInterfaces)
            {
                await _interfaceRepository.AddAsync(iface, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            existingInterfaces = await _interfaceRepository.GetByDeviceIdAsync(device.Id, cancellationToken);
        }

        // Process metric sample update and save history record per interface
        foreach (var iface in existingInterfaces)
        {
            long sampleInOctets = iface.InOctets + 1250000;  // Simulated delta
            long sampleOutOctets = iface.OutOctets + 625000;
            double elapsedSeconds = iface.LastPolledUtc.HasValue ? (nowUtc - iface.LastPolledUtc.Value).TotalSeconds : 60.0;

            double util = InterfaceUtilizationCalculator.CalculateUtilization(
                iface.InOctets,
                sampleInOctets,
                elapsedSeconds,
                iface.SpeedBps);

            iface.UpdateMetrics(
                sampleInOctets,
                sampleOutOctets,
                iface.InErrors,
                iface.OutErrors,
                iface.InDiscards,
                iface.OutDiscards,
                util,
                nowUtc);

            _interfaceRepository.Update(iface);

            var historyRecord = new NetworkInterfaceHistory(
                Guid.NewGuid(),
                iface.TenantId,
                iface.DeviceId,
                iface.Id,
                iface.IfIndex,
                iface.AdminStatus,
                iface.OperStatus,
                iface.SpeedBps,
                sampleInOctets,
                sampleOutOctets,
                iface.InErrors,
                iface.OutErrors,
                iface.InDiscards,
                iface.OutDiscards,
                util,
                nowUtc);

            await _interfaceRepository.AddHistoryAsync(historyRecord, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return existingInterfaces.Select(i => new NetworkInterfaceDto
        {
            Id = i.Id,
            DeviceId = i.DeviceId,
            IfIndex = i.IfIndex,
            Name = i.Name,
            Description = i.Description,
            InterfaceType = i.InterfaceType,
            MacAddress = i.MacAddress,
            SpeedBps = i.SpeedBps,
            AdminStatus = i.AdminStatus,
            OperStatus = i.OperStatus,
            Duplex = i.Duplex,
            InOctets = i.InOctets,
            OutOctets = i.OutOctets,
            InErrors = i.InErrors,
            OutErrors = i.OutErrors,
            InDiscards = i.InDiscards,
            OutDiscards = i.OutDiscards,
            UtilizationPercent = i.UtilizationPercent,
            LastPolledUtc = i.LastPolledUtc
        }).ToList();
    }
}