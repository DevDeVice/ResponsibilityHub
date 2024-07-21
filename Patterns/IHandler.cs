using System.Threading.Tasks;

namespace ResponsibilityHub.Patterns
{
    public interface IHandler<TInput, TOutput>
    {
        Task<TOutput> Handle(TInput message);
    }
}
