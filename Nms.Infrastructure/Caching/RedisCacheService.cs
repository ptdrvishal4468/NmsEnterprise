using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nms.Application.Common.Interfaces;
using StackExchange.Redis;

namespace Nms.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer? _redis;
    private readonly ITenantContext _tenantContext;
    private readonly CacheOptions _options;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _serializerOptions;

    public RedisCacheService(
        ITenantContext tenantContext,
        IOptions<CacheOptions> options,
        ILogger<RedisCacheService> logger,
        IConnectionMultiplexer? redis = null)
    {
        _tenantContext = tenantContext;
        _options = options.Value;
        _logger = logger;
        _redis = redis;

        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private string BuildTenantKey(string key)
    {
        var tenantId = _tenantContext.TenantId == Guid.Empty
            ? "global"
            : _tenantContext.TenantId.ToString("D");

        return $"tenant:{tenantId}:{key.TrimStart(':')}";
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_redis == null || !_redis.IsConnected || !_options.EnableRedis)
        {
            return default;
        }

        try
        {
            var db = _redis.GetDatabase();
            var tenantKey = BuildTenantKey(key);
            var value = await db.StringGetAsync(tenantKey).WaitAsync(cancellationToken);

            if (value.IsNullOrEmpty)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(value.ToString(), _serializerOptions);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis cache retrieval failed for key: {Key}. Gracefully falling back to source.", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        if (_redis == null || !_redis.IsConnected || !_options.EnableRedis || value == null)
        {
            return;
        }

        try
        {
            var db = _redis.GetDatabase();
            var tenantKey = BuildTenantKey(key);
            var expiry = expiration ?? TimeSpan.FromMinutes(_options.DefaultExpirationMinutes);
            var json = JsonSerializer.Serialize(value, _serializerOptions);

            await db.StringSetAsync(tenantKey, json, expiry).WaitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis cache write failed for key: {Key}. Operation skipped safely.", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (_redis == null || !_redis.IsConnected || !_options.EnableRedis)
        {
            return;
        }

        try
        {
            var db = _redis.GetDatabase();
            var tenantKey = BuildTenantKey(key);
            await db.KeyDeleteAsync(tenantKey).WaitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis cache deletion failed for key: {Key}.", key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
    {
        if (_redis == null || !_redis.IsConnected || !_options.EnableRedis)
        {
            return;
        }

        try
        {
            var serverEndpoints = _redis.GetEndPoints();
            if (serverEndpoints.Length == 0) return;

            var pattern = $"{BuildTenantKey(prefixKey)}*";
            var server = _redis.GetServer(serverEndpoints[0]);
            var db = _redis.GetDatabase();

            foreach (var key in server.Keys(pattern: pattern))
            {
                cancellationToken.ThrowIfCancellationRequested();
                await db.KeyDeleteAsync(key).WaitAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis cache prefix deletion failed for prefix: {Prefix}.", prefixKey);
        }
    }
}