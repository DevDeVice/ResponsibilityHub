using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResponsibilityHub.Patterns
{
    public class StorageRepository : IRepository
    {
        private readonly string _connString;
        private readonly string _container;

        public StorageRepository() { }

        public StorageRepository(string connString, string container)
        {
            _connString = connString;
            _container = container;
        }

        public Task Save<T>(T o) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<T> Get<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<T> GetEnumerable<T>() where T : class
        {
            throw new NotImplementedException();
        }
    }
}
