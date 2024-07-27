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
    public class BasicController(StorageConfig config) : Controller
    {
        /*[HttpGet("persons")]
        public Task<Person> Get()
        {
            var storageRepo = Factory.Create<StorageRepository>(config);
            //return storageRepo
            return storageRepo.GetEnumerable<PersonSimple>();
        }*/

        [HttpPost]
        public async Task<IResult> Save([FromBody] PersonRequest request)
        {
            var mapper = new PersonRequestMapper();
            var person = mapper.Map(request);
            var storage = Factory.Create(config);
            await storage.Save(request);
            return Results.Ok();
        }
    }
}
