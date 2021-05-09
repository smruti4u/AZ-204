using System;
using System.Threading.Tasks;

namespace RedisCacheWebApp.Services
{
    public interface ICacheService
    {
        Task<T> GetData<T>(string key);
        Task SetData<T>(string key, T t, TimeSpan timeSpan);
    }
}