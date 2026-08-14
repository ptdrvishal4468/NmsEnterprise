using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Topology.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Topology.Commands.CreateTopologyLink;

public class CreateTopologyLinkCommandHandler : IRequestHandler<CreateTopologyLinkCommand, TopologyLinkDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly INetworkInterfaceRepository _networkInterfaceRepository;

    public CreateTopologyLinkCommandHandler(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext,
        INetworkInterfaceRepository networkInterfaceRepository)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _networkInterfaceRepository = networkInterfaceRepository;
    }

    public async Task<TopologyLinkDto> Handle(CreateTopologyLinkCommand request, CancellationToken cancellationToken)
    {
        var sourceDevice = await _unitOfWork.Devices.GetByIdAsync(request.SourceDeviceId, cancellationToken)
            ?? throw new KeyNotFoundException($"Source device with ID {request.SourceDeviceId} not found.");

        var targetDevice = await _unitOfWork.Devices.GetByIdAsync(request.TargetDeviceId, cancellationToken)
            ?? throw new KeyNotFoundException($"Target device with ID {request.TargetDeviceId} not found.");

        NetworkInterface? sourceInterface = null;
        if (request.SourceInterfaceId.HasValue)
        {
            sourceInterface = await _networkInterfaceRepository.GetByIdAsync(request.SourceInterfaceId.Value, cancellationToken)
                ?? throw new KeyNotFoundException($"Source interface with ID {request.SourceInterfaceId.Value} not found.");

            if (sourceInterface.DeviceId != sourceDevice.Id)
                throw new InvalidOperationException("Source interface does not belong to the source device.");
        }

        NetworkInterface? targetInterface = null;
        if (request.TargetInterfaceId.HasValue)
        {
            targetInterface = await _networkInterfaceRepository.GetByIdAsync(request.TargetInterfaceId.Value, cancellationToken)
                ?? throw new KeyNotFoundException($"Target interface with ID {request.TargetInterfaceId.Value} not found.");

            if (targetInterface.DeviceId != targetDevice.Id)
                throw new InvalidOperationException("Target interface does not belong to the target device.");
        }

        // Check if an existing link exists between these endpoints
        var existingLink = await _unitOfWork.TopologyLinks.FindLinkAsync(
            request.SourceDeviceId,
            request.TargetDeviceId,
            request.LayerType,
            request.Protocol,
            request.SourceInterfaceId,
            request.TargetInterfaceId,
            cancellationToken);

        TopologyLink link;
        if (existingLink != null)
        {
            existingLink.TouchDiscovery(DateTime.UtcNow, request.SpeedBps, request.MetadataJson);
            link = existingLink;
        }
        else
        {
            link = new TopologyLink(
                Guid.NewGuid(),
                _tenantContext.TenantId,
                request.SourceDeviceId,
                request.TargetDeviceId,
                request.LayerType,
                request.Protocol,
                request.SourceInterfaceId,
                request.TargetInterfaceId,
                request.SpeedBps,
                TopologyLinkStatus.Active,
                DateTime.UtcNow,
                request.MetadataJson);

            await _unitOfWork.TopologyLinks.AddAsync(link, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TopologyLinkDto(
            link.Id,
            link.SourceDeviceId,
            sourceDevice.Name,
            link.SourceInterfaceId,
            sourceInterface?.Name,
            link.TargetDeviceId,
            targetDevice.Name,
            link.TargetInterfaceId,
            targetInterface?.Name,
            link.LayerType,
            link.Protocol,
            link.Status,
            link.SpeedBps,
            link.LastDiscoveredUtc,
            link.MetadataJson,
            link.CreatedAtUtc,
            link.CreatedBy);
    }
}