using System.Diagnostics;
using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Dashboard.Dtos;

namespace Nms.Application.Dashboard.Queries.GetPerformanceDashboard;

public sealed class GetPerformanceDashboardQueryHandler : IRequestHandler<GetPerformanceDashboardQuery, PerformanceDashboardDto>
{
    private readonly IPollingQueue _pollingQueue;
    private readonly ICacheService _cacheService;
    private readonly ITenantContext _tenantContext;

    public GetPerformanceDashboardQueryHandler(
        IPollingQueue pollingQueue,
        ICacheService cacheService,
        ITenantContext tenantContext)
    {
        _pollingQueue = pollingQueue;
        _cacheService = cacheService;
        _tenantContext = tenantContext;
    }

    public async Task<PerformanceDashboardDto> Handle(
        GetPerformanceDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var cacheKey = $"dashboard:performance:{tenantId}";

        var cached = await _cacheService.GetAsync<PerformanceDashboardDto>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        using var process = Process.GetCurrentProcess();
        var allocatedMemory = GC.GetTotalMemory(forceFullCollection: false);
        var workingSet = process.WorkingSet64;
        var threadCount = process.Threads.Count;

        var queueCapacity = _pollingQueue.Capacity;
        var queuedCount = _pollingQueue.Count;
        var queueUtilization = queueCapacity > 0
            ? Math.Round(((double)queuedCount / queueCapacity) * 100.0, 2)
            : 0.0;

        var dto = new PerformanceDashboardDto
        {
            TimestampUtc = DateTime.UtcNow,
            ProcessAllocatedMemoryBytes = allocatedMemory,
            WorkingSetBytes = workingSet,
            Gen0Collections = GC.CollectionCount(0),
            Gen1Collections = GC.CollectionCount(1),
            Gen2Collections = GC.CollectionCount(2),
            ThreadCount = threadCount,
            QueueCapacity = queueCapacity,
            QueuedJobsCount = queuedCount,
            QueueUtilizationPercent = queueUtilization,
            CacheStatus = "Connected",
            DatabaseStatus = "Healthy"
        };

        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromSeconds(15), cancellationToken);

        return dto;
    }
}