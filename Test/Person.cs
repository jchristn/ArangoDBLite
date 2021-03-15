using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
    public class Person
    {
        public string FirstName { get; set; } = null;
        public string LastName { get; set; } = null;

        public Person()
        {

        }

        public Person(string first, string last)
        {
            FirstName = first;
            LastName = last;
        }
    }
}
