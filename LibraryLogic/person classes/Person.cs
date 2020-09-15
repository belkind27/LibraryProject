using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace LibraryLogic
{

    public abstract class Person
    {

        private string _name;
        private string _password;
        private bool _isLabrarian;
        public string Name { get { return _name; } set { _name = value; } }
        public string Password { get { return _password; } set { _password = value; } }
        public bool IsLibrarian { get { return _isLabrarian; } set { _isLabrarian = value; } }
        protected Person(string name, string password)
        {
            if (int.TryParse(name, out int i)) throw new LibrarySystemException("name cant be a number");
            _name = name;
            _password = password;
        }
        public Person() 
        {

        }
        public override string ToString()
        {
            return $"name: {_name} password: {_password}";
        }


    }

}
