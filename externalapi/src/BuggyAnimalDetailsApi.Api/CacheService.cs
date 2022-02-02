using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace BuggyAnimalDetailsApi.Api;

public class CacheService
{
    private const string LastRequestKey = "LAST_REQUEST";
    private const int CoolingDownSeconds = 30;
    private const int MaxNumberOfItemsInCache = 10;
    public const int ExpireItemsInSeconds = 15;

    private readonly CoolingDownApiFlag _coolingDownFlag;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(CoolingDownApiFlag coolingDownFlag, IMemoryCache memoryCache, ILogger<CacheService> logger)
    {
        _coolingDownFlag = coolingDownFlag;
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public bool TryAddRequestToCache(object value)
    {
        var cacheCount = ((MemoryCache) _memoryCache).Count;
        _logger.LogInformation($"CacheCount: {cacheCount}");
        
        if (_coolingDownFlag.Enabled && _memoryCache.TryGetValue(LastRequestKey, out DateTime allowCallsAfter) && allowCallsAfter > DateTime.Now)
        {
            _logger.LogInformation("Extending cooling down period. slow down!");
            _memoryCache.Set(LastRequestKey, DateTime.Now.AddSeconds(CoolingDownSeconds));
            return false;
        }

        if (cacheCount >= MaxNumberOfItemsInCache)
        {
            if (_coolingDownFlag.Enabled)
                _memoryCache.Set(LastRequestKey, DateTime.Now.AddSeconds(CoolingDownSeconds));

            return false;
        }

        var expirationTime = DateTime.Now.AddSeconds(ExpireItemsInSeconds);
        var expirationToken = new CancellationChangeToken(new CancellationTokenSource(TimeSpan.FromSeconds(ExpireItemsInSeconds + .01)).Token);
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetPriority(CacheItemPriority.NeverRemove)
            .SetAbsoluteExpiration(expirationTime)
            .AddExpirationToken(expirationToken)
            .RegisterPostEvictionCallback(callback: CacheItemRemoved, state: this);

        _memoryCache.Set(Guid.NewGuid(), value, cacheEntryOptions);
        return true;
    }

    private void CacheItemRemoved(object key, object value, EvictionReason reason, object state)
    {
        _logger.LogInformation($"Item {value} is {reason}.");
    }
}