using Microsoft.AspNetCore.Mvc;
using ResponsibilityHub.Models;
using ResponsibilityHub.Patterns;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResponsibilityHub.Controllers;

public record PersonRequest(Guid Id, string Name, string Surname, string Pesel);

[ApiController]
[Route("basic-route")]
public class BasicController : Controller
{
    private readonly StorageConfig _config;

    public BasicController(StorageConfig config)
    {
        _config = config;
    }

    [HttpGet("persons")]
    public IAsyncEnumerable<Person> Get()
    {
        //var storageRepo = Factory.Create<StorageRepository>(config);
        var storageRepo = Factory.Create(_config);
        return storageRepo.GetEnumerable<Person.Simple>();
    }

    [HttpGet("all-persons")]
    public async Task<List<Person>> GetAll()
    {
        var storageRepo = Factory.Create(_config);
        return await storageRepo.GetAll<Person>();
    }

    [HttpPost]
    public async Task<IResult> Save([FromBody] PersonRequest request)
    {
        var mapper = new PersonRequestMapper();
        var person = mapper.Map(request);
        var storage = Factory.Create(_config);
        await storage.Save(person);
        return Results.Ok();
    }
}
