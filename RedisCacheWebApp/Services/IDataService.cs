using System.Threading.Tasks;

namespace RedisCacheWebApp.Services
{
    public interface IDataService
    {
        Task<State> GetData(int id);
    }
}