using ResponsibilityHub.DDD.Base;
using ResponsibilityHub.DDD.Infrastructure;
using ResponsibilityHub.Models;
using ResponsibilityHub.Patterns;
using static ResponsibilityHub.DDD.AppointmentAggregate.AppointmentRepository;

namespace ResponsibilityHub.DDD.AppointmentAggregate;

public abstract class AppointmentEvent : EventArgs
{
    public class New : AppointmentEvent
    {
        public Guid Id { get; }
        public Slot Slot { get; }

        public New(Guid id, Slot slot)
        {
            Id = id;
            Slot = slot;
        }
    }

    public class Canceled : AppointmentEvent
    {
        public Guid Id { get; }

        public Canceled(Guid id)
        {
            Id = id;
        }
    }
}
public record struct Slot(DateTime Start, DateTime End);
public class AppointmentApp : Publisher, IAggregateRoot //value object,struct - slot / kto jest zapiety na domene moze zrealizowac eventy
{
    private readonly IRepository<AppointmentApp> _repository;
    private readonly AppointmentInfrastructure _infrastructure;

    public AppointmentApp(IRepository<AppointmentApp> repository, AppointmentInfrastructure infrastructure)
    {
        _repository = repository;
        _infrastructure = infrastructure;
        _infrastructure.Subscribe(this); 
    }

    public Guid Id { get; private set; }
    public Slot Slot { get; private set; }
    public Person Patient { get; private set; }
    public AppointmentStatus Status { get; private set; } = AppointmentStatus.Scheduled;

    public AppointmentApp(Guid id, Slot slot, Person patient)
    {
        Id = id;
        Slot = slot;
        Patient = patient;
    }

    public void Create(Slot slot, Person patient, Action<Guid> onSuccess, Action<string> onError)
    {
        if (slot.Start > DateTime.UtcNow.AddDays(1))
        {
            Id = Guid.NewGuid();
            Slot = slot;
            Patient = patient;
            Status = AppointmentStatus.Scheduled;

            var ev = new AppointmentEvent.New(Id, Slot);
            Publish(ev);

            onSuccess(Id);
        }
        else
        {
            onError("Failed: Appointment must be scheduled at least 1 day in advance.");
        }
    }

    public void Cancel(Action onSuccess, Action<string> onError)
    {
        if (Status == AppointmentStatus.Scheduled)
        {
            Status = AppointmentStatus.Canceled;
            var ev = new AppointmentEvent.Canceled(Id);
            Publish(ev);
            onSuccess?.Invoke();
        }
        else
        {
            onError?.Invoke("Failed: Only scheduled appointments can be canceled.");
        }
    }
    public void Reschedule(Slot newSlot, Action onSuccess, Action<string> onError)
    {
        if (Status == AppointmentStatus.Scheduled)
        {
            if (newSlot.Start > DateTime.UtcNow.AddDays(1))
            {
                Slot = newSlot;
                var ev = new AppointmentEvent.New(Id, Slot);
                Publish(ev);

                onSuccess();
            }
            else
            {
                onError("Failed: New appointment time must be at least 1 day in advance.");
            }
        }
        else
        {
            onError("Failed: Only scheduled appointments can be rescheduled.");
        }
    }
    public bool IsOverlapping(Slot otherSlot)
    {
        return Slot.Start < otherSlot.End && Slot.End > otherSlot.Start;
    }
}
//ServiceBus pokazuje tylko co wrzucone do bloba (Tu eventy domenowe do rozbudowania) -

//jeden Decider - rozbudowac,

//drugi DDD - event 1/2 maja blad - zbudowac domene biznesowa 4:22:20
//domena sama weryfikuje / repozytorium i zapis do bazy
//stworzyc takie stany agregatu aby pokrywały sie z eventami
//person validowany w konstruktorze 
//np. domena waliduje czy mamy do czynienia z slotem w przeszlosci i rzucic error czy umowic
//zrobic wiecej polityk wewnatrz