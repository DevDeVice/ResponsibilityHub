using System.Text.Json.Serialization;

namespace ResponsibilityHub.Models;

[JsonDerivedType(typeof(Simple), "simple")]
[JsonDerivedType(typeof(WithPesel), "withPesel")]
public abstract record Person(Guid Id)
{

    public record Simple(Guid Id, string Name, string Surname) : Person(Id);

    public record WithPesel(Guid Id, string Name, string Surname, string Pesel) : Person(Id);

}
