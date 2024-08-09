namespace ResponsibilityHub.DDD.Base;

public interface IAggregateRoot
{
    public Guid Id { get; }
}

public interface IRepository<T> where T : IAggregateRoot
{
    void Add(T entity);
    T GetById(Guid id);
    void Update(T entity);
    void Delete(Guid id);
    IEnumerable<T> GetAll();
}