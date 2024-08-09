using Microsoft.AspNetCore.Mvc;
using ResponsibilityHub.DDD.AppointmentAggregate;
using ResponsibilityHub.Models;

[ApiController]
[Route("api/appointments")]
public class AppointmentController : ControllerBase
{
    private readonly AppointmentApp _appointmentApp;

    public AppointmentController(AppointmentApp appointmentApp)
    {
        _appointmentApp = appointmentApp;
    }

    [HttpPost]
    public IActionResult CreateAppointment([FromBody] Slot slot, [FromBody] Person patient)
    {
        _appointmentApp.Create(slot, patient,
            onSuccess: (id) => Ok(new { AppointmentId = id }),
            onError: (message) => BadRequest(message));

        return Ok();
    }

    [HttpPost("{id:guid}/cancel")]
    public IActionResult CancelAppointment(Guid id)
    {
        _appointmentApp.Cancel(
            onSuccess: () => NoContent(),
            onError: (message) => BadRequest(message));

        return NoContent();
    }
}
