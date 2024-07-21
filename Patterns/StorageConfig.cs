namespace ResponsibilityHub.Patterns
{
    public record StorageConfig(string ConnString, string Container) : IConfigurationFor<StorageRepository>;
}
