using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResponsibilityHub.Patterns
{
    public enum RepoType { Storage, Cosmos }

    public interface IRepository
    {
        Task Save<T>(T o) where T : class;
        Task<T> Get<T>() where T : class;
        IAsyncEnumerable<T> GetEnumerable<T>() where T : class;
    }
}
