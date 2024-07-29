using ResponsibilityHub.Models;
using ResponsibilityHub.Controllers;

namespace ResponsibilityHub.Patterns;
public interface IMapper<in Input, out Output> where Input : class where Output : class
{
    public Output Map(Input input);
}
public class PersonRequestMapper : IMapper<PersonRequest, Person>
{
    public Person Map(PersonRequest input)
    {
        if (string.IsNullOrEmpty(input.Pesel) == true)
        {
            return new Person.Simple(input.Id, input.Name, input.Surname);
        }
        else
        {
            return new Person.WithPesel(input.Id, input.Name, input.Surname, input.Pesel);
        }
    }
}