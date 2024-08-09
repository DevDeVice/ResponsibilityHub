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
        var builder = new PersonBuilder()
                        .WithId(input.Id)
                        .WithName(input.Name)
                        .WithSurname(input.Surname);

        if (!string.IsNullOrWhiteSpace(input.Pesel))
        {
            builder.WithPesel(input.Pesel);
        }

        return builder.Build();
    }
}