using ResponsibilityHub.Models;
using ResponsibilityHub.Controllers;

namespace ResponsibilityHub.Patterns
{
    public class Mapper : IMapper<PersonRequest, Person>
    {
        public Person Map(PersonRequest input)
        {
            if (string.IsNullOrEmpty(input.Pesel))
            {
                return new PersonSimple(input.Id, input.Name, input.Surname);
            }
            else
            {
                return new PersonWithPesel(input.Id, input.Name, input.Surname, input.Pesel);
            }
        }
    }
}