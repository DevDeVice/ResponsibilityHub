using Microsoft.Azure.Amqp.Framing;
using System.Collections.Concurrent;

namespace ResponsibilityHub.Decider;

public abstract record Event(Guid Id, DateTime OccurredAt)
{
    public record Booked(Guid Id, DateTime Started, DateTime Ended, DateTime OccurredAt) : Event(Id, OccurredAt);
    public record Canceled(Guid Id, DateTime OccurredAt) : Event(Id, OccurredAt);
    public record Rescheduled(Guid Id, DateTime NewStart, DateTime NewEnd, DateTime OccurredAt) : Event(Id, OccurredAt);
}
public abstract record Command
{
    public record Create(DateTime Start, DateTime End) : Command;
    public record Canceled : Command;
    public record Reschedule(DateTime NewStart, DateTime NewEnd) : Command;
}
public abstract record SlotState
{
    public record Initial : SlotState; 
    public sealed record Booked(Guid Id, DateTime Start, DateTime End) : SlotState;
    public  record Free(Guid Id) : SlotState;
}

public static class Decider
{
    public sealed record Slot(Guid Id, List<SlotState.Booked> Bookings);
    public static ConcurrentQueue<Event> events = new();
    private static SlotState Evolve(SlotState state, Event ev) => (state, ev) switch
    {
        (SlotState.Initial, Event.Booked evb) => new SlotState.Booked(Guid.NewGuid(),evb.Started,evb.Ended),
        (SlotState.Free ssf, Event.Booked evb) => new SlotState.Booked(ssf.Id, evb.Started, evb.Ended),
        (SlotState.Booked ssb, Event.Canceled) => new SlotState.Free(ssb.Id),
        (SlotState.Booked ssb, Event.Rescheduled er) => new SlotState.Booked(ssb.Id, er.NewStart, er.NewEnd),
        _ => state
    };
    
    public static IEnumerable<Event> Decide(this SlotState state, Command command) => (state, command) switch
    {
        (SlotState.Initial, Command.Create cc) => Book(cc.Start, cc.End),
        (SlotState.Free, Command.Create cc) => Book(cc.Start, cc.End),
        (SlotState.Booked ssb, Command.Canceled cc) => Cancel(),
        (SlotState.Booked ssb, Command.Reschedule cr) => Reschedule(cr.NewStart, cr.NewEnd),
        _ => throw new InvalidOperationException("Invalid command for the current state.")
    };
    public static SlotState Fold(this IEnumerable<Event> events, SlotState state) => events.Aggregate(state, Evolve);
    public static SlotState Fold(this IEnumerable<Event> events) => events.Fold(new SlotState.Initial());

    private static IEnumerable<Event> Book(DateTime start, DateTime end)
    {
        if (end <= start)
        {
            throw new ArgumentException("End time must be after start time.");
        }
        if (start <= DateTime.UtcNow.AddHours(24))
        {
            throw new InvalidOperationException("Reservations must be made at least 24 hours in advance.");
        }
        var ev = new Event.Booked(Guid.NewGuid(), start, end, DateTime.UtcNow);
        events.Enqueue(ev);
        return events;
    }
    private static IEnumerable<Event> Cancel()
    {
        var ev = new Event.Canceled(Guid.NewGuid(), DateTime.UtcNow);
        events.Enqueue(ev);
        return events;
    }
    private static IEnumerable<Event> Reschedule(DateTime newStart, DateTime newEnd)
    {
        if (newEnd <= newStart)
        {
            throw new ArgumentException("End time must be after start time.");
        }
        if (newStart <= DateTime.UtcNow.AddHours(24))
        {
            throw new InvalidOperationException("Reservations must be made at least 24 hours in advance.");
        }
        var ev = new Event.Rescheduled(Guid.NewGuid(), newStart, newEnd, DateTime.UtcNow);
        events.Enqueue(ev);
        return events;
    }
}
