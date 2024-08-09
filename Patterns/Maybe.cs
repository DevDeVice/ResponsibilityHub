namespace ResponsibilityHub.Patterns;

public abstract class Maybe<T>
{
    public abstract Maybe<T1> Map<T1>(Func<T, T1> f);
    public abstract TResult MatchWith<TResult>((Func<TResult> None, Func<T, TResult> Some) pattern);
    public Maybe<T1> Bind<T1>(Func<T, Maybe<T1>> f) => MatchWith((
        none: () => new None<T1>(),
        some: (v) => f(v)
        ));
}
public class None<T> : Maybe<T>
{
    public override Maybe<T1> Map<T1>(Func<T, T1> f) => new None<T1>();

    public override TResult MatchWith<TResult>((Func<TResult> None, Func<T, TResult> Some) pattern) => pattern.None();
}
public class Some<T> : Maybe<T>
{
    private readonly T value;
    public Some(T value)
    {
        this.value = value;
    }
    public override Maybe<T1> Map<T1>(Func<T, T1> f) => new Some<T1>(f(value));

    public override TResult MatchWith<TResult>((Func<TResult> None, Func<T, TResult> Some) pattern) => pattern.Some(value);
}