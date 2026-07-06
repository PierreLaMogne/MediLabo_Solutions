using Microsoft.Extensions.Caching.Memory;

namespace MediLabo_Solutions.Frontend.Services
{
    public class HttpCacheService(IMemoryCache cache) : IHttpCacheService
    {
        private readonly HashSet<string> _cacheKeys = new();
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        /// <summary>
        /// Créer ou récupérer une valeur du cache. Si la valeur n'existe pas, elle sera créée en utilisant la fonction factory fournie.
        /// </summary>
        /// <typeparam name="T">Le type de la valeur à mettre en cache</typeparam>
        /// <param name="key">La clé du cache</param>
        /// <param name="factory">La fonction pour créer la valeur si elle n'existe pas dans le cache</param>
        /// <param name="absoluteExpiration">La durée d'expiration absolue de l'entrée du cache</param>
        /// <returns>La valeur mise en cache ou créée</returns>
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
                    SlidingExpiration = TimeSpan.FromMinutes(2),
                    PostEvictionCallbacks =
                    {
                        new PostEvictionCallbackRegistration
                        {
                            EvictionCallback = (key, value, reason, state) =>
                            {
                                _cacheKeys.Remove(key.ToString()!);
                            }
                        }
                    }
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
