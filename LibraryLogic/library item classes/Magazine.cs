using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryLogic
{
    public class Magazine : LibraryItem
    {
        private int _edition;
        public int Edition { get { return _edition; } set { _edition = value; } }
        public Magazine(int edition, string name, string publisher, Genres genre, double price, DateTime dateOfPrinting, int id,int amount) : base(name, publisher, genre, price, dateOfPrinting, id, amount)
        {
            _edition = edition;
        }
        public Magazine() : base()
        {

        }
        public override string ToString()
        {
            return "magazine " + base.ToString() + $" edition: {Edition}";

        }

    }

}
