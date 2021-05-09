using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisCacheWebApp.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase database;

        private const string connectionString = "az-204rediscache.redis.cache.windows.net:6380,password=V68rLfD7at8CHx9fB3VzgSXfSnxRJGBDqGLnWL7dakA=,ssl=True,abortConnect=False";
        public CacheService()
        {
            database = GetDatabase(connectionString);
        }

        private IDatabase GetDatabase(string connectionString)
        {
            var lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(connectionString);
            });

            return lazyConnection.Value.GetDatabase();
        }

        public async Task SetData<T>(string key, T t, TimeSpan timeSpan)
        {
            var json = JsonConvert.SerializeObject(t);
            await this.database.StringSetAsync(key, json, timeSpan);
        }

        public async Task<T> GetData<T>(string key)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));

            var result = await this.database.StringGetAsync(key)
                .ConfigureAwait(false);

            return (result == RedisValue.Null) ? default(T) :
                JsonConvert.DeserializeObject<T>(result);

        }
    }
}
