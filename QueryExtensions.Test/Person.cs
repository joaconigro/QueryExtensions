using System;
using System.Collections.Generic;
using System.Linq;

namespace QueryExtensions.Test
{
    internal class Person
    {
        public Person(int id, string name, string surname, double height, DateTime birthdate, DateTime lastAccess, PersonRole role)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Height = height;
            Birthdate = birthdate;
            LastAccess = lastAccess;
            Role = role;
            Age = (int)((DateTime.Today - birthdate).TotalDays / 365);
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }
        public DateTime Birthdate { get; set; }
        public DateTime LastAccess { get; set; }
        public PersonRole Role { get; set; }

        internal static List<Person> People()
        {
            return new List<Person>
            {
                new Person(1, "John", "Smith", 1.85, new DateTime(1969, 6, 3), new DateTime(2020, 12, 3, 12, 15, 3), PersonRole.User),
                new Person(2, "Arthur", "Besse", 1.66, new DateTime(1972, 9, 19), new DateTime(2020, 10, 18, 17, 31, 3), PersonRole.User),
                new Person(3, "Peter", "Orno", 1.93, new DateTime(1969, 5, 3), new DateTime(2020, 5, 3, 12, 15, 3), PersonRole.User),
                new Person(4, "Jára", "Cimrman", 1.52, new DateTime(1985, 6, 14), new DateTime(2019, 6, 14, 12, 15, 3), PersonRole.Admin),
                new Person(5, "Ponsonby", "Britt", 1.69, new DateTime(1988, 2, 16), new DateTime(2020, 2, 16, 12, 15, 3), PersonRole.User),
                new Person(6, "George", "Spelvin", 1.73, new DateTime(1991, 9, 18), new DateTime(2019, 9, 18, 12, 15, 3), PersonRole.User),
                new Person(7, "Andreas", "Karavis", 1.45, new DateTime(1979, 10, 9), new DateTime(2020, 10, 9, 12, 15, 3), PersonRole.Admin),
                new Person(8, "Kozma", "Prutkov", 1.85, new DateTime(1978, 11, 12), new DateTime(2021, 11, 12, 12, 15, 3), PersonRole.User),
                new Person(9, "Penelope", "Ashe", 1.89, new DateTime(1983, 1, 10), new DateTime(2021, 1, 10, 12, 15, 3), PersonRole.User),
                new Person(10, "Allegra", "Coleman", 1.74, new DateTime(1982, 8, 21), new DateTime(2021, 8, 21, 12, 15, 3), PersonRole.User),
            };
        }

        internal static IQueryable<Person> PeopleQuery()
        {
            return People().AsQueryable();
        }
    }

    internal enum PersonRole
    {
        User,
        Admin
    }
}
