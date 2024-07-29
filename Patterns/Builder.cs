using ResponsibilityHub.Models;

namespace ResponsibilityHub.Patterns;

public class PersonSimpleBuilder
{
    private string name, surname;

    public PersonSimpleBuilder AddName(string name) {
        {
            this.name = name;
            return this;
        } }
    public PersonSimpleBuilder AddSurname(string surname)
    {
        this.surname = surname;
        return this;
    }
    public Person Build()
    {
        var person = new Person.Simple(Guid.NewGuid(), name, surname);
        return person;
    }
}