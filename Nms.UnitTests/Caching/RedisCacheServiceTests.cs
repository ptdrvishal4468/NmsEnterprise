using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Infrastructure.Caching;
using StackExchange.Redis;
using Xunit;

namespace Nms.UnitTests.Caching;

public class RedisCacheServiceTests
{
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly Mock<IOptions<CacheOptions>> _optionsMock;
    private readonly Mock<ILogger<RedisCacheService>> _loggerMock;
    private readonly CacheOptions _cacheOptions;

    public RedisCacheServiceTests()
    {
        _tenantContextMock = new Mock<ITenantContext>();
        _optionsMock = new Mock<IOptions<CacheOptions>>();
        _loggerMock = new Mock<ILogger<RedisCacheService>>();

        _cacheOptions = new CacheOptions
        {
            DefaultExpirationMinutes = 5,
            DashboardExpirationSeconds = 30,
            RuleCacheExpirationMinutes = 15,
            EnableRedis = true
        };

        _optionsMock.Setup(o => o.Value).Returns(_cacheOptions);
    }

    [Fact]
    public async Task GetAsync_WhenRedisMultiplexerIsNull_ReturnsDefaultGracefully()
    {
        var tenantId = Guid.NewGuid();
        _tenantContextMock.Setup(t => t.TenantId).Returns(tenantId);

        var cacheService = new RedisCacheService(
            _tenantContextMock.Object,
            _optionsMock.Object,
            _loggerMock.Object,
            redis: null);

        var result = await cacheService.GetAsync<string>("test-key");

        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_WhenRedisMultiplexerIsNull_CompletesWithoutThrowing()
    {
        var tenantId = Guid.NewGuid();
        _tenantContextMock.Setup(t => t.TenantId).Returns(tenantId);

        var cacheService = new RedisCacheService(
            _tenantContextMock.Object,
            _optionsMock.Object,
            _loggerMock.Object,
            redis: null);

        var exception = await Record.ExceptionAsync(() =>
            cacheService.SetAsync("test-key", new { Data = "test-value" }));

        Assert.Null(exception);
    }

    [Fact]
    public async Task RemoveAsync_WhenRedisIsDisabled_CompletesWithoutThrowing()
    {
        _cacheOptions.EnableRedis = false;
        var tenantId = Guid.NewGuid();
        _tenantContextMock.Setup(t => t.TenantId).Returns(tenantId);

        var cacheService = new RedisCacheService(
            _tenantContextMock.Object,
            _optionsMock.Object,
            _loggerMock.Object,
            redis: null);

        var exception = await Record.ExceptionAsync(() =>
            cacheService.RemoveAsync("test-key"));

        Assert.Null(exception);
    }

    [Fact]
    public async Task RemoveByPrefixAsync_WhenRedisMultiplexerIsNull_CompletesWithoutThrowing()
    {
        var tenantId = Guid.NewGuid();
        _tenantContextMock.Setup(t => t.TenantId).Returns(tenantId);

        var cacheService = new RedisCacheService(
            _tenantContextMock.Object,
            _optionsMock.Object,
            _loggerMock.Object,
            redis: null);

        var exception = await Record.ExceptionAsync(() =>
            cacheService.RemoveByPrefixAsync("dashboard"));

        Assert.Null(exception);
    }
}