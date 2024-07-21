namespace ResponsibilityHub.Models
{
    public abstract record Person(Guid Id, string Name, string Surname);

    public record PersonSimple(Guid Id, string Name, string Surname) : Person(Id, Name, Surname);

    public record PersonWithPesel(Guid Id, string Name, string Surname, string Pesel) : Person(Id, Name, Surname);
}
