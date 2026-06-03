using Microsoft.Extensions.Caching.Memory;

public class CacheService
{
    private readonly IMemoryCache _cache;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public T GetOrCreate<T>(string key, Func<ICacheEntry, T> createItem)
    {
        return _cache.GetOrCreate(key, createItem);
    }
}   