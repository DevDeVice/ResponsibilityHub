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

public abstract class Validator<T> where T: class
{
    private List<ISpecification<T>> specs = new List<ISpecification<T>>();
    public Validator<T> Add(ISpecification<T> specification)
    {
        specs.Add(specification);
        return this;
    }
    public T Execute(T input)
    {
        foreach(var item in specs)
        {
            if (item.IsSatisfiedBy(input) == false)
                throw new Exception(); //TODO reszta
        }
        return input;
    }
}

public class PersonValidator : Validator<Person>;