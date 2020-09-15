using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryLogic
{
    public abstract class LibraryItem
    {
        private string _name;
        private string _publisher;
        private Genres _genre;
        private double _price;
        private DateTime _dateOfPrinting;
        private int _id;
        private int _amount;

        public string Name { get { return _name; } set { _name = value; } }
        public string Publisher { get { return _publisher; } set { _publisher = value; } }
        public Genres Genre { get { return _genre; } set { _genre = value; } }
        public double Price { get { return _price; } set { _price = value; } }
        public DateTime DateOfPrinting { get { return _dateOfPrinting; } set { _dateOfPrinting = value; } }
        public int Id { get { return _id; } set { _id = value; } }
        public int Amount { get { return _amount; } set { _amount = value; } }
        protected LibraryItem(string name, string publisher, Genres genre, double price, DateTime dateOfPrinting, int id,int amount)
        {
            if (int.TryParse(name, out int i)) throw new LibrarySystemException("name cant be a number");
            _name = name;
            if (int.TryParse(publisher, out  i)) throw new LibrarySystemException("name cant be a number");
            _publisher = publisher;
            _genre = genre;
            _price = price;
            _dateOfPrinting = dateOfPrinting;
            _id = id;
            _amount = amount;
        }
        protected LibraryItem()
        { }
        public override string ToString()
        {
            return $"name: {Name} publisher: {Publisher} genre: {Genre} price: {Price} amount: {Amount}";
        }

    }

}
