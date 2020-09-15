using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryLogic
{
    public abstract class Sale : IknowDateDeadLine
    {
        private string _name;
        private int _precentageOfDicrease;
        private DateTime _endDate;
        public string Name { get { return _name; } set { _name = value; } }
        public int PrecentageOfDicrease { get { return _precentageOfDicrease; } set { _precentageOfDicrease = value; } }
        public DateTime EndDate { get { return _endDate; } set { _endDate = value; } }
        public Sale(string name, int precentageOfDicrease, DateTime dateTime)
        {
            if (int.TryParse(name, out int i)) throw new LibrarySystemException("name cant be a number");
            _name = name;
            _precentageOfDicrease = precentageOfDicrease;
            _endDate = dateTime;
        }
        public virtual double itemOnSaleCheck(LibraryItem item)
        {
            double i = 0;
            return i;
        }
        protected double PriceChange(double currentPrice)
        {

            double p;
            p = (currentPrice *(100 - PrecentageOfDicrease)) / 100;
            return p;
        }
        public bool IsActive()
        {
            DateTime dateTime = DateTime.Now;
            if (EndDate.Year <= dateTime.Year && EndDate.DayOfYear <= dateTime.DayOfYear)
            {
                return false;
            }
            else return true;
        }
        public Sale()
        { }

    }

}
