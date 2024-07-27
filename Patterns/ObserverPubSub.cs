namespace ResponsibilityHub.Patterns;

public class CustomEvent : EventArgs;
public class Publisher
{
    public event EventHandler<CustomEvent> CustomEvent;
    public void Publish()
    {
        CustomEvent?.Invoke(this, new CustomEvent());
        //OR
        foreach(var handler in CustomEvent.GetInvocationList())
        {
            handler.DynamicInvoke(this, new CustomEvent());
        }
    }
}
public class Subscriber
{
    public void Subscribe(Publisher publisher)
    {
        publisher.CustomEvent += onEventReised;
    }

    private void onEventReised(object? sender, CustomEvent e)
    {
        //TODO - Jakas logika u subskrybenta
        throw new NotImplementedException();
    }
}