using ResponsibilityHub.Models;

namespace ResponsibilityHub.Patterns;

public class PersonBuilder
{
    private string _name;
    private string _surname;
    private string _pesel;
    private Guid _id;

    public PersonBuilder()
    {
        _id = Guid.NewGuid();
    }

    public PersonBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public PersonBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public PersonBuilder WithSurname(string surname)
    {
        _surname = surname;
        return this;
    }

    public PersonBuilder WithPesel(string pesel)
    {
        _pesel = pesel;
        return this;
    }

    public Person Build()
    {
        if (!string.IsNullOrWhiteSpace(_pesel))
        {
            return new Person.WithPesel(_id, _name, _surname, _pesel);
        }
        else
        {
            return new Person.Simple(_id, _name, _surname);
        }
    }
    /*private string name, surname;
    public PersonBuilder AddName(string name) {
        {
            this.name = name;
            return this;
        } }
    public PersonBuilder AddSurname(string surname)
    {
        this.surname = surname;
        return this;
    }
    public Person Build()
    {
        var person = new Person.Simple(Guid.NewGuid(), name, surname);
        return person;
    }*/
}