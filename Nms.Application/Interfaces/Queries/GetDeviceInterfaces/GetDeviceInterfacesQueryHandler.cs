using MediatR;
using Nms.Application.Interfaces.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Interfaces.Queries.GetDeviceInterfaces;

public class GetDeviceInterfacesQueryHandler : IRequestHandler<GetDeviceInterfacesQuery, IReadOnlyList<NetworkInterfaceDto>>
{
    private readonly INetworkInterfaceRepository _interfaceRepository;

    public GetDeviceInterfacesQueryHandler(INetworkInterfaceRepository interfaceRepository)
    {
        _interfaceRepository = interfaceRepository;
    }

    public async Task<IReadOnlyList<NetworkInterfaceDto>> Handle(GetDeviceInterfacesQuery request, CancellationToken cancellationToken)
    {
        var interfaces = await _interfaceRepository.GetByDeviceIdAsync(request.DeviceId, cancellationToken);

        return interfaces.Select(i => new NetworkInterfaceDto
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