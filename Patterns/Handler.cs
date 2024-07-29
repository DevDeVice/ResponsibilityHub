using ResponsibilityHub.Models;
using ResponsibilityHub.Controllers;

namespace ResponsibilityHub.Patterns;

public interface IHandler<Input, Output>
{
    Task<Output> Handle(Input message);
}
public class PersonRequestHandler : IHandler<PersonRequest, Person>
{
    public async Task<Person> Handle(PersonRequest message)
    {
        PersonRequestMapper mapper = new();
        var person = mapper.Map(message);
        //todo logika
        return person;
    }
}