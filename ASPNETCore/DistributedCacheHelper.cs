using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace ASPNETCore
{
    public class DistributedCacheHelper : IDistributedCacheHelper
    {
        private readonly IDistributedCache distributedCache;

        public DistributedCacheHelper(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        private static DistributedCacheEntryOptions CreateOptions(int baseExpireSeconds)
        {
            double seconds = Random.Shared.NextDouble(baseExpireSeconds, baseExpireSeconds * 2);
            TimeSpan expiration = TimeSpan.FromSeconds(seconds);
            DistributedCacheEntryOptions options = new()
            {
                AbsoluteExpirationRelativeToNow = expiration
            };
            return options;
        }

        public TResult? GetOrCreate<TResult>(string cacheKey, Func<DistributedCacheEntryOptions, TResult?> valueFactory, int expireSeconds = 60)
        {
            var jsonStr = distributedCache.GetString(cacheKey);
            //缓存中不存在
            if (string.IsNullOrEmpty(jsonStr))
            {
                var options = CreateOptions(expireSeconds);
                TResult? result = valueFactory(options);//如果数据源中也没有查到，可能会返回null
                //null会被json序列化为字符串"null"，所以可以防范“缓存穿透”
                string jsonOfResult = JsonSerializer.Serialize(result,
                    typeof(TResult));
                distributedCache.SetString(cacheKey, jsonOfResult, options);
                return result;
            }
            else
            {
                //"null"会被反序列化为null
                //TResult如果是引用类型，就有为null的可能性；如果TResult是值类型
                //在写入的时候肯定写入的是0、1之类的值，反序列化出来不会是null
                //所以如果obj这里为null，那么存进去的时候一定是引用类型
                distributedCache.Refresh(cacheKey);//刷新，以便于滑动过期时间延期
                return JsonSerializer.Deserialize<TResult>(jsonStr)!;
            }
        }

        public async Task<TResult?> GetOrCreateAsync<TResult>(string cacheKey, Func<DistributedCacheEntryOptions, Task<TResult?>> valueFactory, int expireSeconds = 60)
        {
            var jsonStr = await distributedCache.GetStringAsync(cacheKey);
            if (string.IsNullOrEmpty(jsonStr))
            {
                var options = CreateOptions(expireSeconds);
                TResult? result = await valueFactory(options);
                string jsonOfResult = JsonSerializer.Serialize(result,
                    typeof(TResult));
                await distributedCache.SetStringAsync(cacheKey, jsonOfResult, options);
                return result;
            }
            else
            {
                await distributedCache.RefreshAsync(cacheKey);
                return JsonSerializer.Deserialize<TResult>(jsonStr)!;
            }
        }

        public void Remove(string cacheKey)
        {
            distributedCache.Remove(cacheKey);
        }

        public Task RemoveAsync(string cacheKey)
        {
            return distributedCache.RemoveAsync(cacheKey);
        }
    }
}
