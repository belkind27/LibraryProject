using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryLogic
{
    public class Book : LibraryItem
    {
        private string _auther;
        public string Auther { get { return _auther; } set { _auther = value; } }
        public Book(string auther, string name, string publisher, Genres genre, double price, DateTime dateOfPrinting, int id,int amount) : base(name, publisher, genre, price, dateOfPrinting, id, amount)
        {
            if (int.TryParse(auther, out int i)) throw new LibrarySystemException("name cant be a number");
            _auther = auther;
        } 
        public Book() : base()
        { 
        
        }
        public override string ToString()
        {
            return "book " + base.ToString() + $" auther: {Auther}";
        }


    }

}
