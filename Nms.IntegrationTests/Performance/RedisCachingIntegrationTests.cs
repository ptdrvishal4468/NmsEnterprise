using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Nms.Application.Common.Interfaces;
using Nms.Infrastructure.Caching;
using StackExchange.Redis;
using Xunit;

namespace Nms.IntegrationTests.Performance;

public class RedisCachingIntegrationTests
{
    private sealed class TestTenantContext : ITenantContext
    {
        public Guid TenantId { get; set; } = Guid.NewGuid();
        public bool IsResolved { get; set; } = true;
    }

    [Fact]
    public async Task RedisCacheService_WhenRedisIsRunning_SetsAndGetsValueSuccessfully()
    {
        // Arrange
        var tenantContext = new TestTenantContext();
        var cacheOptions = Options.Create(new CacheOptions
        {
            DefaultExpirationMinutes = 5,
            EnableRedis = true
        });
        var logger = NullLogger<RedisCacheService>.Instance;

        // Connect to local Redis if available; skip test execution if running without external Redis
        IConnectionMultiplexer? multiplexer = null;
        try
        {
            multiplexer = await ConnectionMultiplexer.ConnectAsync("localhost:6379,abortConnect=true,connectTimeout=1000");
        }
        catch (RedisConnectionException)
        {
            // Redis is not running in this test environment — graceful exit
            return;
        }

        using (multiplexer)
        {
            var cacheService = new RedisCacheService(
                tenantContext,
                cacheOptions,
                logger,
                multiplexer);

            var testKey = $"test-key-{Guid.NewGuid()}";
            var testPayload = new Dictionary<string, string>
            {
                ["Message"] = "IntegrationTest",
                ["Timestamp"] = DateTime.UtcNow.ToString("O")
            };

            // Act & Assert Set/Get
            await cacheService.SetAsync(testKey, testPayload, TimeSpan.FromSeconds(30));
            var retrieved = await cacheService.GetAsync<Dictionary<string, string>>(testKey);

            Assert.NotNull(retrieved);
            Assert.Equal("IntegrationTest", retrieved["Message"]);

            // Act & Assert Remove
            await cacheService.RemoveAsync(testKey);
            var afterDelete = await cacheService.GetAsync<Dictionary<string, string>>(testKey);

            Assert.Null(afterDelete);
        }
    }
    [Fact]
    public async Task RedisCacheService_WhenRedisIsOffline_GracefullyReturnsDefaultWithoutThrowing()
    {
        // Arrange
        var tenantContext = new TestTenantContext();
        var cacheOptions = Options.Create(new CacheOptions
        {
            DefaultExpirationMinutes = 5,
            EnableRedis = true
        });
        var logger = NullLogger<RedisCacheService>.Instance;

        // Explicitly pass null multiplexer to simulate disconnected/offline state
        var cacheService = new RedisCacheService(
            tenantContext,
            cacheOptions,
            logger,
            redis: null);

        var testKey = $"offline-key-{Guid.NewGuid()}";

        // Act & Assert
        var result = await cacheService.GetAsync<Dictionary<string, string>>(testKey);
        Assert.Null(result);

        var exception = await Record.ExceptionAsync(() =>
            cacheService.SetAsync(testKey, new Dictionary<string, string> { ["Key"] = "Value" }));

        Assert.Null(exception);
    }
}