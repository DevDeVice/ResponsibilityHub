namespace ResponsibilityHub.Patterns
{
    public static class Factory
    {
        public static IRepository Get(RepoType repoType, IConfigurationFor<IRepository> config)
        {
            return repoType switch
            {
                RepoType.Storage => config switch
                {
                    StorageConfig sc => new StorageRepository(sc.ConnString, sc.Container),
                    _ => throw new NotImplementedException()
                },
                RepoType.Cosmos => config switch
                {
                    CosmosConfig cc => new CosmosRepository(),
                    _ => throw new NotImplementedException()
                },
                _ => throw new NotImplementedException()
            };
        }

        public static T Create<T>(IConfigurationFor<T> config) where T : class, IRepository
        {
            IRepository repository = null;
            switch (config)
            {
                case StorageConfig sc:
                    repository = new StorageRepository(sc.ConnString, sc.Container);
                    break;
                case CosmosConfig cc:
                    repository = new CosmosRepository();
                    break;
            }
            return repository as T;
        }
    }
}
