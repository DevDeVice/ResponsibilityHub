namespace ResponsibilityHub.DDD.Base;

public interface IAggregateRoot
{
    public Guid Id { get; }
}

public interface IRepository<T> where T : IAggregateRoot;