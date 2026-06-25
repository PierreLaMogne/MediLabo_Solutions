using Microsoft.Extensions.Caching.Memory;

namespace MediLabo_Solutions.Frontend.Services
{
    public class HttpCacheService(IMemoryCache cache) : IHttpCacheService
    {
        private readonly HashSet<string> _cacheKeys = new();
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? absoluteExpiration = null)
        {
            if (cache.TryGetValue(key, out T? cachedValue))
            {
                return cachedValue;
            }
            await _semaphore.WaitAsync();
            try
            {
                if (cache.TryGetValue(key, out cachedValue))
                {
                    return cachedValue;
                }
                var value = await factory();
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = absoluteExpiration ?? TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };
                cache.Set(key, value, cacheEntryOptions);
                _cacheKeys.Add(key);
                return value;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Remove(string key)
        {
            cache.Remove(key);
            _cacheKeys.Remove(key);
        }

        public void RemoveByPrefix(string prefix)
        {
            var keysToRemove = _cacheKeys.Where(k => k.StartsWith(prefix)).ToList();
            foreach (var key in keysToRemove)
            {
                cache.Remove(key);
                _cacheKeys.Remove(key);
            }
        }

        public void Clear()
        {
            foreach (var key in _cacheKeys.ToList())
            {
                cache.Remove(key);
                _cacheKeys.Remove(key);
            }
            _cacheKeys.Clear();
        }
    }
}
