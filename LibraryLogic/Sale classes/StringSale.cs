using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryLogic
{
    public class StringSale : Sale
    {
        private string _word;
        private int _filter;
        public string Word { get { return _word; } set { _word = value; } }
        public int Filter { get { return _filter; } set { _filter = value; } }//0=name 1=auther 2=publisher
        public StringSale(string word, int filter, string name, int precentageOfDicrease, DateTime dateTime) : base(name, precentageOfDicrease, dateTime)
        {
            if (int.TryParse(word, out int i)) throw new LibrarySystemException("word cant be a number");
            _word = word;
            _filter = filter;
        }
        public StringSale() : base()
        { }
        public override double itemOnSaleCheck(LibraryItem item)
        {
            double Price;
            switch (Filter)
            {
                case 0:
                    if (item.Name.Contains( Word)) Price = PriceChange(item.Price);
                    else Price = 10000000; break;
                case 1:
                    Book book = item as Book;
                    if (book.Auther == Word) Price = PriceChange(item.Price);
                    else Price = 10000000; break;
                case 2:
                    if (item.Publisher == Word) Price = PriceChange(item.Price);
                    else Price = 10000000; break;
                default: Price = 10000000; break;
            }
            return Price;
        }
        public override string ToString()
        {
            string s;
            if (Filter == 0) s = $" items name {Word}";
            else if (Filter == 1) s = $" items written by {Word}";
            else s = $" items published by {Word}";
            return $"{Name} Sale, {PrecentageOfDicrease}% off" + s;
        }
    }

}
