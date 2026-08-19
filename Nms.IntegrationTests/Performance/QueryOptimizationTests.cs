using Microsoft.EntityFrameworkCore;
using Nms.Application.Common.Interfaces;
using Nms.Infrastructure.Data;
using Xunit;

namespace Nms.IntegrationTests.Performance;

public class QueryOptimizationTests
{
    private sealed class TestTenantContext(Guid tenantId) : ITenantContext
    {
        public Guid TenantId { get; } = tenantId;
        public bool IsResolved => true;
    }

    [Fact]
    public async Task TenantFilteredRead_UsesNoTrackingAndPagination()
    {
        var tenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<NmsDbContext>()
            .UseInMemoryDatabase($"performance-{Guid.NewGuid()}")
            .Options;

        await using var context = new NmsDbContext(options, new TestTenantContext(tenantId));
        await context.Database.EnsureCreatedAsync();

        var profiles = await context.PollProfiles
            .AsNoTracking()
            .Where(profile => profile.TenantId == tenantId)
            .OrderBy(profile => profile.Name)
            .Skip(0)
            .Take(10)
            .ToListAsync();

        Assert.Empty(profiles);
        Assert.False(context.ChangeTracker.Entries().Any());
    }
}
