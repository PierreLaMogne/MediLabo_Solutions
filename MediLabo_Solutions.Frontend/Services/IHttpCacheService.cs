namespace MediLabo_Solutions.Frontend.Services
{
    public interface IHttpCacheService
    {
        Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? absoluteExpiration = null);
        void Remove(string key);
        void RemoveByPrefix(string prefix);
        void Clear();
    }
}
