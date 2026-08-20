using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Dashboard.Dtos;
using Nms.Application.Dashboard.Queries.GetPerformanceDashboard;
using Xunit;

namespace Nms.UnitTests.Observability;

public sealed class PerformanceDashboardQueryHandlerTests
{
    private readonly Mock<IPollingQueue> _mockPollingQueue;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<ITenantContext> _mockTenantContext;
    private readonly GetPerformanceDashboardQueryHandler _handler;

    public PerformanceDashboardQueryHandlerTests()
    {
        _mockPollingQueue = new Mock<IPollingQueue>();
        _mockCacheService = new Mock<ICacheService>();
        _mockTenantContext = new Mock<ITenantContext>();

        _mockTenantContext.Setup(t => t.TenantId).Returns(Guid.NewGuid());

        _handler = new GetPerformanceDashboardQueryHandler(
            _mockPollingQueue.Object,
            _mockCacheService.Object,
            _mockTenantContext.Object);
    }

    [Fact]
    public async Task Handle_WhenCached_ShouldReturnCachedDataWithoutComputing()
    {
        var cachedDto = new PerformanceDashboardDto
        {
            ProcessAllocatedMemoryBytes = 1048576,
            QueuedJobsCount = 10,
            QueueCapacity = 100,
            CacheStatus = "Connected"
        };

        _mockCacheService
            .Setup(c => c.GetAsync<PerformanceDashboardDto>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedDto);

        var result = await _handler.Handle(new GetPerformanceDashboardQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1048576, result.ProcessAllocatedMemoryBytes);
        Assert.Equal(10, result.QueuedJobsCount);
        _mockCacheService.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<PerformanceDashboardDto>(), It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenNotCached_ShouldAggregateRuntimeStatsAndCache()
    {
        _mockCacheService
            .Setup(c => c.GetAsync<PerformanceDashboardDto>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PerformanceDashboardDto?)null);

        _mockPollingQueue.Setup(q => q.Capacity).Returns(1000);
        _mockPollingQueue.Setup(q => q.Count).Returns(50);

        var result = await _handler.Handle(new GetPerformanceDashboardQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.ProcessAllocatedMemoryBytes > 0);
        Assert.Equal(1000, result.QueueCapacity);
        Assert.Equal(50, result.QueuedJobsCount);
        Assert.Equal(5.0, result.QueueUtilizationPercent);

        _mockCacheService.Verify(c => c.SetAsync(
            It.IsAny<string>(),
            It.IsAny<PerformanceDashboardDto>(),
            It.Is<TimeSpan?>(t => t.HasValue && t.Value.TotalSeconds == 15),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}