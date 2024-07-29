using ResponsibilityHub.Models;
namespace ResponsibilityHub.Patterns;

public interface ISpecification<T> where T : class
{
    public bool IsSatisfiedBy(T input);
}
public class PersonSimpleSpecification : ISpecification<Person>
{
    public bool IsSatisfiedBy(Person input)
    {
        return true;
    }
}

public class PersonWithPeselSpecification : ISpecification<Person>
{
    public bool IsSatisfiedBy(Person input)
    {
        return input is Person.WithPesel person && IsValidPesel(person.Pesel);
    }
    private bool IsValidPesel(string pesel)
    {
        //todo kod walidujacy pesel
        return true;
    }
}