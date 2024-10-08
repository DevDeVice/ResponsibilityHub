﻿using Microsoft.AspNetCore.Mvc;
using ResponsibilityHub.Extensions;
using ResponsibilityHub.Models;
using ResponsibilityHub.Patterns;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ResponsibilityHub.Controllers;
public record PersonRequest(Guid Id, string Name, string Surname, string? Pesel);

[ApiController]
[Route("basic-route")]
public class BasicController : Controller
{
    private readonly StorageConfig _config;

    public BasicController(StorageConfig config)
    {
        _config = config;
    }

    [HttpGet("persons/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var storageRepo = Factory.Create(_config);
        var person = await storageRepo.Get<Person>(id);

        if (person == null)
        {
            return NotFound();
        }
        return Ok(person);
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

        await mapper
            .Then((m) => m.Map(request))
            .Then((p) => new PersonValidator()
                .Add(new PersonSimpleSpecification())
                .Execute(p))
            .Then(async(p) => Factory.Create(_config).Save(p));

        /*var person = mapper.Map(request);
        var storageRepo = Factory.Create(_config);
        await storageRepo.Save(person);*/
        return Results.Ok();
    }

    [HttpPut("persons/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PersonRequest request)
    {
        var storageRepo = Factory.Create(_config);
        var person = await storageRepo.Get<Person>(id);

        if (person == null)
        {
            return NotFound();
        }

        // Mapowanie żądania na istniejącą osobę
        var mapper = new PersonRequestMapper();
        var updatedPerson = mapper.Map(request);

        await storageRepo.Save(updatedPerson);

        return Ok(updatedPerson);
    }

    [HttpDelete("persons/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var storageRepo = Factory.Create(_config);
        var person = await storageRepo.Get<Person>(id);
        if (person == null)
        {
            return NotFound();
        }
        await storageRepo.Delete<Person>(id);
        return NoContent();
    }

    [HttpDelete("delete-all")]
    public async Task<IResult> DeleteAll()
    {
        var storageRepo = Factory.Create(_config);
        await storageRepo.DeleteAll<Person>();
        return Results.Ok();
    }
    
}
