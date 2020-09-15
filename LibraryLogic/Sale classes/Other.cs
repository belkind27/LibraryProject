using System;
using System.Collections.Generic;

namespace LibraryLogic
{
    public enum Genres
    {
        ActionAndAdventure,

        ArtAndArchitecture,

        AlternateHistory,

        Autobiography,

        Anthology,

        Biography,

        BusinessAndEconomics,

        Childrens,

        CraftsAndHobbies,

        Cookbook,

        ComicBook,

        Diary,

        Dictionary,

        Crime,

        Encyclopedia,

        Drama,

        Guide,

        Fairytale,

        HealthAndFitness,

        Fantasy,

        History,

        GraphicNovel,

        HomeAndGarden,

        HistoricalFiction,

        Humor,

        Horror,

        Journal,

        Mystery,

        Math,

        Memoir,

        PictureBook,

        Philosophy,

        Poetry,

        Prayer,

        Romance,

        Textbook,

        Satire,

        TrueCrime,

        ScienceFiction,

        Review,

        ShortStory,

        Science,

        Suspense,

        SelfHelp,

        Thriller,

        SportsAndLeisure,

        Western,

        Travel,

    }
    public interface IknowDateDeadLine
    {
        bool IsActive();

    }
    public class Borrow : IknowDateDeadLine
    {
        private LibraryItem _item;
        private DateTime _whenBorrowHappend;
        private DateTime _whenBorrowEnds;
        private double _price;
        private bool _isBorrowActive;
        private bool _isLate;
        private int _clientsId;
        public LibraryItem Item { get { return _item; } set { _item = value; } }
        public DateTime WhenBorrowHappend { get { return _whenBorrowHappend; } set { _whenBorrowHappend = value; } }
        public DateTime WhenBorrowEnds { get { return _whenBorrowEnds; } set { _whenBorrowEnds = value; } }
        public double Price { get { return _price; } set { _price = value; } }
        public bool IsBorrowActive { get { return _isBorrowActive; } set { _isBorrowActive = value; } }
        public bool IsLate { get { return _isLate; } set { _isLate = value; } }
        public int ClientsId { get { return _clientsId; } set { _clientsId = value; } }
        public Borrow(LibraryItem item, DateTime whenBorrowHappend,int id)
        {
            _item = item;
            _whenBorrowHappend = whenBorrowHappend;
            _whenBorrowEnds = whenBorrowHappend.AddDays(14);
            _isBorrowActive = true;
            _price = item.Price;
            _clientsId = id;
        }
        public Borrow() { }
        public void Delay()
        {
            DateTime dateTime = DateTime.Now;
            if (IsBorrowActive && WhenBorrowEnds.Year <= dateTime.Year && WhenBorrowEnds.DayOfYear <= dateTime.DayOfYear)
            {
                IsLate = true;
            }
            else IsLate = false;

        }
        public bool IsActive()
        {
            return IsBorrowActive;
        }
        public override string ToString()
        {
            Book book = Item as Book;
            if (book == null)
            {
                Magazine magazine = Item as Magazine;
                return $"you borrowd the {Enum.GetName(typeof(Genres), (int)magazine.Genre)} magazine {magazine.Name} edition {magazine.Edition} " +
                    $"Published by {magazine.Publisher} at the price of {magazine.Price} in {WhenBorrowHappend.ToShortDateString()}";

            }
            else return $"you borrowd the {Enum.GetName(typeof(Genres), (int)book.Genre)} book {book.Name} written by {book.Auther} " +
                  $"Published by {book.Publisher} at the price of {book.Price} in {WhenBorrowHappend.ToShortDateString()}";

        }
        public string ReportView()
        {
            Book book = Item as Book;
            if (book == null)
            {
                Magazine magazine = Item as Magazine;
                return $"magazine name: {magazine.Name} genre: {Enum.GetName(typeof(Genres), (int)magazine.Genre)}" +
                    $" edition: {magazine.Edition} publisher: {magazine.Publisher} price: {magazine.Price} happend: {WhenBorrowHappend} end: {WhenBorrowEnds}";
            }
            else return $"magazine name: {book.Name} genre: {Enum.GetName(typeof(Genres), (int)book.Genre)}" +
            $" auther: {book.Auther} publisher: {book.Publisher} price: {book.Price} happend: {WhenBorrowHappend} end: {WhenBorrowEnds}";
        }
        
    }
    public class LibrarySystemException : Exception
    {
        public LibrarySystemException() { }
        public LibrarySystemException(string message) : base(message) { }
        public LibrarySystemException(string message, Exception inner) : base(message, inner) { }
        protected LibrarySystemException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}
