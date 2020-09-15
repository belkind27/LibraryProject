using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryLogic
{
    public class Librarian : Person
    {
        public Librarian(string Name, string password) : base(Name, password)
        {
            IsLibrarian = true;
        }
        public Librarian() : base()
        {

        }
        public override string ToString()
        {
            return "librarian "+base.ToString();
        }
    }

}
