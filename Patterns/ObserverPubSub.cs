namespace ResponsibilityHub.Patterns;

public abstract class Publisher
{
    public event EventHandler Handler;
    public void Publish(EventArgs args)
    {
        Handler.Invoke(this, args);
    }
}
public abstract class Subscriber
{
    public void Subscribe(Publisher publisher)
    {
        publisher.Handler += onEventReised;
    }

    public abstract void onEventReised(object? sender, EventArgs e);
}