using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryLogic
{
    public class GenreSale : Sale
    {
        private Genres _genre;
        public Genres Genre { get { return _genre; } set { _genre = value; } }
        public GenreSale(Genres genre, string name, int precentageOfDicrease, DateTime dateTime) : base(name, precentageOfDicrease, dateTime)
        {
            _genre = genre;
        }
        public GenreSale() : base()
        { }
        public override double itemOnSaleCheck(LibraryItem item)
        {
            double Price;
            if (item.Genre == Genre) Price = PriceChange(item.Price);
            else Price = 10000000;
            return Price;

        }
        public override string ToString()
        {
            return $"{Name} Sale, {PrecentageOfDicrease}% off {Genre} items";
        }
    }

}
