namespace ResponsibilityHub.Patterns;

public abstract record ConfigurationFor<T> where T : IRepository;
public record StorageConfig(string ConnString, string Container) : ConfigurationFor<StorageRepository>;
//public record CosmosConfig() : ConfigurationFor<CosmosRepository>;//TODO CosmosConfig