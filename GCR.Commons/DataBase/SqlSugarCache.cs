using SqlSugar;
using SqlSugar.Extensions;

namespace GCR.Commons
{
    /// <summary>
    /// SqlSugar缓存类
    /// </summary>
    public class SqlSugarCache : ICacheService
    {
        private readonly ICache cache;

        /// <summary>
        /// SqlSugar缓存类
        /// </summary>
        /// <param name="cache"></param>
        public SqlSugarCache()
        {
            cache = new MemoryCacheHelper();
        }

        public void Add<V>(string key, V value)
        {
            cache.Set(key, value);
        }

        public void Add<V>(string key, V value, int cacheDurationInSeconds)
        {
            cache.Set(key, value, cacheDurationInSeconds);
        }

        public bool ContainsKey<V>(string key)
        {
            return cache.Exists(key);
        }

        public V Get<V>(string key)
        {
            return cache.Get<V>(key);
        }

        public IEnumerable<string> GetAllKey<V>()
        {
            return cache.GetCacheKeys();
        }

        public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
        {
            if (cache.Exists(cacheKey))
            {
                return cache.Get<V>(cacheKey);
            }
            else
            {
                try
                {
                    var result = create();
                    var second = DateTime.Now.AddSeconds(cacheDurationInSeconds).Second;
                    while (second == 0)
                    {
                        second = DateTime.Now.AddSeconds(cacheDurationInSeconds).Second;
                    }
                    cache.Set(cacheKey, result, second);
                    return result;
                }
                catch { return default; }
            }
        }

        public void Remove<V>(string key)
        {
            cache.Remove(key);
        }
    }
}
