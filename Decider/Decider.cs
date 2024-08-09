using Microsoft.Azure.Amqp.Framing;
using System.Collections.Concurrent;

namespace ResponsibilityHub.Decider;

public abstract record Event(Guid Id, DateTime OccurredAt)
{
    //TODO rozbudowac event - wizyta(przeszlosc - nie mozna anulowac), przeniesienie (np. 24h wczesniej)
    public record Booked(Guid Id, DateTime Started, DateTime Ended, DateTime OccurredAt) : Event(Id, OccurredAt);
    public record Canceled(Guid Id, DateTime OccurredAt) : Event(Id, OccurredAt);
}
public abstract record Command
{
    public record Create(DateTime Start, DateTime End) : Command;
    public record Canceled : Command;
}
public abstract record SlotState
{
    //Inny stan poczatkowy - Posłużyć sie Deciderem - wytworzyć to, zrobić projekt testowy - pokazac jak tego uzywamy
    public record Initial : SlotState; 
    public sealed record Booked(Guid Id, DateTime Start, DateTime End) : SlotState;
    public  record Free(Guid Id) : SlotState;
}

public static class Decider
{
    public static ConcurrentQueue<Event> events = new();
    private static SlotState Evolve(SlotState state, Event ev) => (state, ev) switch
    {
        (SlotState.Initial, Event.Booked evb) => new SlotState.Booked(Guid.NewGuid(),evb.Started,evb.Ended),
        (SlotState.Free ssf, Event.Booked evb) => new SlotState.Booked(ssf.Id, evb.Started, evb.Ended),
        (SlotState.Booked ssb, Event.Canceled) => new SlotState.Free(ssb.Id),
        _ => state
    };
    
    public static IEnumerable<Event> Decide(this SlotState state, Command command) => (state, command) switch
    {
        (SlotState.Initial, Command.Create cc) => Book(cc.Start, cc.End),
        (SlotState.Free, Command.Create cc) => Book(cc.Start, cc.End),
        (SlotState.Booked ssb, Command.Canceled cc) => Cancel(),
        _ => throw new NotImplementedException() //TODO dorobic błąd domenowy
    };
    public static SlotState Fold(this IEnumerable<Event> events, SlotState state) => events.Aggregate(state, Evolve);
    public static SlotState Fold(this IEnumerable<Event> events) => events.Fold(new SlotState.Initial());

    private static IEnumerable<Event> Book(DateTime start, DateTime end)
    {
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
}
