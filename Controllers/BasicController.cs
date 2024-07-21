using Microsoft.AspNetCore.Mvc;
using ResponsibilityHub.Models;
using ResponsibilityHub.Patterns;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResponsibilityHub.Controllers
{
    public record PersonRequest(Guid Id, string Name, string Surname, string Pesel);

    [ApiController]
    [Route("basic-route")]
    public class BasicController : ControllerBase
    {
        [HttpGet("persons")]
        public async IAsyncEnumerable<Person> Get()
        {
            var sc = new StorageConfig("", "");
            var storageRepo = Factory.Create<StorageRepository>(sc);
            await foreach (var person in storageRepo.GetEnumerable<PersonSimple>())
            {
                yield return person;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] PersonRequest personRequest)
        {
            var handler = new PersonRequestHandler();
            var person = await handler.Handle(personRequest);
            // Save person logic here
            return Ok(person);
        }
    }
}
