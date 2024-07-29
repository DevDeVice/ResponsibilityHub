namespace ResponsibilityHub.Patterns
{
    public static class Factory
    {
        public static IRepository Get(RepoType repoType)/*, IConfigurationFor<IRepository> config*/
        {
            /*return repoType switch
            {
                RepoType.Storage => config switch
                {
                    StorageConfig sc => new Repository(sc.ConnString, sc.Container),
                    _ => throw new NotImplementedException()
                },
                RepoType.Cosmos => config switch
                {
                    CosmosConfig cc => new CosmosRepository(),
                    _ => throw new NotImplementedException()
                },
                _ => throw new NotImplementedException()
            };*/
            return repoType switch
            {
                RepoType.Storage => new StorageRepository(),
                _ => throw new ArgumentOutOfRangeException(nameof(repoType), repoType, null)
            };
            /*IRepository repository = repoType switch
            {
                RepoType.Storage => new StorageRepository(),
                *//*RepoType.Cosmos => new CosmosRepository(),*//*
                _ => throw new NotImplementedException(),
            };*/
        }

        public static T Create<T>(ConfigurationFor<T> config) where T : class, IRepository
        {
            IRepository repository = null;
            switch (config)
            {
                case StorageConfig sc:
                    repository = new StorageRepository(sc.ConnString, sc.Container);
                    break;
                /*case CosmosConfig cc:
                    repository = new CosmosRepository();
                    break;*/
            }
            return repository as T;
        }
    }
}
