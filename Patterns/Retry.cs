namespace ResponsibilityHub.Patterns;

public class RetryHelper
{
    public static async Task<T> Retry<T>(Func<Task<T>> action, TimeSpan retryInterval, int retryCount)
    {
        try
        {
            return await action();
        } catch when (retryCount > 0)
        {
            await Task.Delay(retryInterval);
            return await Retry(action, retryInterval, --retryCount);
        }
    }

    public static async Task Retry(Func<Task> action, TimeSpan retryInterval, int retryCount)
    {
        try
        {
            await action();
        } catch when (retryCount > 0)
        {
            await Task.Delay(retryInterval);
            await Retry(action, retryInterval, --retryCount);
        }
    }
}
