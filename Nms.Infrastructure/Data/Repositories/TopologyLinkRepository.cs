using Microsoft.EntityFrameworkCore;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Infrastructure.Data.Repositories;

public class TopologyLinkRepository : GenericRepository<TopologyLink, Guid>, ITopologyLinkRepository
{
    public TopologyLinkRepository(NmsDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<TopologyLink>> GetLinksAsync(
        TopologyLayerType? layerType = null,
        TopologyLinkStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(t => t.SourceDevice)
            .Include(t => t.TargetDevice)
            .Include(t => t.SourceInterface)
            .Include(t => t.TargetInterface)
            .AsNoTracking()
            .AsQueryable();

        if (layerType.HasValue)
        {
            query = query.Where(t => t.LayerType == layerType.Value || t.LayerType == TopologyLayerType.Both);
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TopologyLink>> GetDeviceLinksAsync(
        Guid deviceId,
        TopologyLayerType? layerType = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(t => t.SourceDevice)
            .Include(t => t.TargetDevice)
            .Include(t => t.SourceInterface)
            .Include(t => t.TargetInterface)
            .AsNoTracking()
            .Where(t => t.SourceDeviceId == deviceId || t.TargetDeviceId == deviceId);

        if (layerType.HasValue)
        {
            query = query.Where(t => t.LayerType == layerType.Value || t.LayerType == TopologyLayerType.Both);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TopologyLink?> FindLinkAsync(
        Guid sourceDeviceId,
        Guid targetDeviceId,
        TopologyLayerType layerType,
        LinkDiscoveryProtocol protocol,
        Guid? sourceInterfaceId = null,
        Guid? targetInterfaceId = null,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(t =>
                ((t.SourceDeviceId == sourceDeviceId && t.TargetDeviceId == targetDeviceId &&
                  t.SourceInterfaceId == sourceInterfaceId && t.TargetInterfaceId == targetInterfaceId) ||
                 (t.SourceDeviceId == targetDeviceId && t.TargetDeviceId == sourceDeviceId &&
                  t.SourceInterfaceId == targetInterfaceId && t.TargetInterfaceId == sourceInterfaceId)) &&
                t.LayerType == layerType &&
                t.Protocol == protocol,
                cancellationToken);
    }

    public async Task<IReadOnlyList<TopologyLink>> GetStaleLinksAsync(
        DateTime olderThanUtc,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(t => t.Status == TopologyLinkStatus.Active && t.LastDiscoveredUtc < olderThanUtc)
            .ToListAsync(cancellationToken);
    }
}