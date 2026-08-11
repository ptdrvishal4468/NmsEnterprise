using MediatR;
using Nms.Application.Interfaces.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Interfaces.Queries.GetInterfaceHistory;

public class GetInterfaceHistoryQueryHandler : IRequestHandler<GetInterfaceHistoryQuery, IReadOnlyList<NetworkInterfaceHistoryDto>>
{
    private readonly INetworkInterfaceRepository _interfaceRepository;

    public GetInterfaceHistoryQueryHandler(INetworkInterfaceRepository interfaceRepository)
    {
        _interfaceRepository = interfaceRepository;
    }

    public async Task<IReadOnlyList<NetworkInterfaceHistoryDto>> Handle(GetInterfaceHistoryQuery request, CancellationToken cancellationToken)
    {
        var histories = await _interfaceRepository.GetHistoryAsync(request.NetworkInterfaceId, request.StartUtc, request.EndUtc, cancellationToken);

        return histories.Select(h => new NetworkInterfaceHistoryDto
        {
            Id = h.Id,
            DeviceId = h.DeviceId,
            NetworkInterfaceId = h.NetworkInterfaceId,
            IfIndex = h.IfIndex,
            AdminStatus = h.AdminStatus,
            OperStatus = h.OperStatus,
            SpeedBps = h.SpeedBps,
            InOctets = h.InOctets,
            OutOctets = h.OutOctets,
            UtilizationPercent = h.UtilizationPercent,
            TimestampUtc = h.TimestampUtc
        }).ToList();
    }
}