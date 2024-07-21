namespace ResponsibilityHub.Patterns
{
    public interface IMapper<in TInput, out TOutput>
        where TInput : class
        where TOutput : class
    {
        TOutput Map(TInput input);
    }
}
