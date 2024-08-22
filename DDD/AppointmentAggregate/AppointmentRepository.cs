using ResponsibilityHub.DDD.Base;
using ResponsibilityHub.Models;

namespace ResponsibilityHub.DDD.AppointmentAggregate;

public class AppointmentRepository : IRepository<AppointmentApp>
{
    public enum AppointmentStatus
    {
        Scheduled,
        Canceled,
        Completed
    }
    private readonly List<AppointmentApp> _appointments = new();

    public void Add(AppointmentApp appointment)
    {
        if (_appointments.Any(a => a.Id == appointment.Id))
        {
            throw new InvalidOperationException("Appointment with the same ID already exists.");
        }
        _appointments.Add(appointment);
    }

    public AppointmentApp GetById(Guid id)
    {
        return _appointments.FirstOrDefault(a => a.Id == id);
    }

    public void Update(AppointmentApp appointment)
    {
        var existingAppointment = GetById(appointment.Id);
        if (existingAppointment != null)
        {
            _appointments.Remove(existingAppointment);
            _appointments.Add(appointment);
        }
        else
        {
            throw new KeyNotFoundException("Appointment not found.");
        }
    }

    public void Delete(Guid id)
    {
        var appointment = GetById(id);
        if (appointment != null)
        {
            _appointments.Remove(appointment);
        }
        else
        {
            throw new KeyNotFoundException("Appointment not found.");
        }
    }

    public IEnumerable<AppointmentApp> GetAll()
    {
        return _appointments;
    }

    public IEnumerable<AppointmentApp> FindByPatient(Person patient)
    {
        return _appointments.Where(a => a.Patient.Id == patient.Id).ToList();
    }
}