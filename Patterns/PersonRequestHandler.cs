using ResponsibilityHub.Controllers;
using ResponsibilityHub.Models;
using System.Threading.Tasks;

namespace ResponsibilityHub.Patterns
{
    public class PersonRequestHandler : IHandler<PersonRequest, Person>
    {
        public async Task<Person> Handle(PersonRequest message)
        {
            var mapper = new Mapper();
            var person = mapper.Map(message);
            // Simulate some async operation
            await Task.CompletedTask;
            return person;
        }
    }
}
