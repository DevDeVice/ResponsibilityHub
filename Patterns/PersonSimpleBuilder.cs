using ResponsibilityHub.Models;

namespace ResponsibilityHub.Patterns
{
    public class PersonSimpleBuilder
    {
        private string _name, _surname;

        public PersonSimpleBuilder AddName(string name)
        {
            _name = name;
            return this;
        }

        public PersonSimpleBuilder AddSurname(string surname)
        {
            _surname = surname;
            return this;
        }

        public Person Build()
        {
            return new PersonSimple(Guid.NewGuid(), _name, _surname);
        }
    }
}
