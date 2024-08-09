using ResponsibilityHub.DDD.Base;
using ResponsibilityHub.Models;
using ResponsibilityHub.Patterns;

namespace ResponsibilityHub.DDD.AppointmentAggregate;

public abstract class AppointmentEvent : EventArgs
{
    public class New(Guid Id, Slot Slot) : AppointmentEvent
    {

    }

    public class Canceled(Guid Id) : AppointmentEvent
    {

    }
}
public record struct Slot(DateTime Start, DateTime End);
public class AppointmentApp : Publisher, IAggregateRoot //value object,struct - slot / kto jest zapiety na domene moze zrealizowac eventy
{
    public Guid Id => throw new NotImplementedException();

    public void Create(Slot slot, Person patient, Action<Guid> onSuccess, Action<string> onError)
    {
        if (slot.Start < DateTime.UtcNow.AddDays(1))
        {
            var ev = new AppointmentEvent.New(Guid.NewGuid(), slot);
            Publish(ev);
            onSuccess(Id);
        } 
        else
        {
            onError("Failed");
        }
    }

    public void Cancel()
    {

    }
}
//ServiceBus pokazuje tylko co wrzucone do bloba (Tu eventy domenowe do rozbudowania) - jeden Decider
