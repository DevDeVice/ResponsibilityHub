namespace ResponsibilityHub.Patterns;

public interface IContext
{
    T GetFromCache<T>() where T : class;
    void PutToCache<T>(T input) where T : class;
}

public interface IOperation<T> where T : class, IContext
{
    Task Execute(T context, IPipe<T> next);
}

public interface IPipe<T> where T : class, IContext
{
    Task Execute(T context);
}

public class Pipe<T> : IPipe<T> where T : class, IContext
{
    private IOperation<T> operation;
    private IPipe<T> pipe;

    public Pipe(IOperation<T> operation, IPipe<T> pipe)
    {
        this.operation = operation;
        this.pipe = pipe;
    }

    public Task Execute(T context)
    {
        return operation.Execute(context, pipe);
    }
}

public class PipeBuilder<T> where T : class, IContext
{
    List<IOperation<T>> operations = new();
    public void AddOperation(IOperation<T> operation)
    {
        operations.Add(operation);
    }
    public IPipe<T> Build()
    {
        IPipe<T> current = null;
        for(int i = 0; i < operations.Count; i++)
            current = new Pipe<T>(operations[i], current);
        return current;
    }
}