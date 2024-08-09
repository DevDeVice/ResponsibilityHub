using ResponsibilityHub.DDD.AppointmentAggregate;
using ResponsibilityHub.Patterns;

namespace ResponsibilityHub.DDD.Infrastructure;

public class AppointmentInfrastructure : Subscriber
{
    private readonly IAppointmentRepository _appointmentRepository;

    public AppointmentInfrastructure(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public override void onEventReised(object? sender, EventArgs e)
    {
        if (e is AppointmentEvent.New newEvent)
        {
            HandleNewAppointment(newEvent);
        }
        else if (e is AppointmentEvent.Canceled canceledEvent)
        {
            HandleCanceledAppointment(canceledEvent);
        }
    }

    private void HandleNewAppointment(AppointmentEvent.New newEvent)
    {
        // Tworzenie nowego spotkania i zapisanie go w repozytorium
        var appointment = new Appointment
        {
            Id = newEvent.Id,
            Slot = newEvent.Slot,
            Status = "Scheduled"
        };

        _appointmentRepository.Save(appointment);
        Console.WriteLine($"New appointment saved with ID: {newEvent.Id}");
    }

    private void HandleCanceledAppointment(AppointmentEvent.Canceled canceledEvent)
    {
        // Znalezienie istniejącego spotkania i aktualizacja jego statusu na anulowany
        var appointment = _appointmentRepository.GetById(canceledEvent.Id);
        if (appointment != null)
        {
            appointment.Status = "Canceled";
            _appointmentRepository.Update(appointment);
            Console.WriteLine($"Appointment with ID: {canceledEvent.Id} has been canceled.");
        }
        else
        {
            Console.WriteLine($"Appointment with ID: {canceledEvent.Id} not found.");
        }
    }
}

// Przykładowa implementacja interfejsu repozytorium
public interface IAppointmentRepository
{
    void Save(Appointment appointment);
    Appointment GetById(Guid id);
    void Update(Appointment appointment);
}

// Klasa Appointment, reprezentująca spotkanie
public class Appointment
{
    public Guid Id { get; set; }
    public Slot Slot { get; set; }
    public string Status { get; set; }
}