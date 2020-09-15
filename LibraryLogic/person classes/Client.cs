using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryLogic
{
    public class Client : Person
    {
        
        private double _balance;
        private bool _isBorrowing;
        private int _id;
        public double Balance { get {return _balance; } set {_balance=value; } }
        public bool IsBorrowing { get { return _isBorrowing; } set { _isBorrowing = value; } }
        public int Id { get { return _id; } set { _id = value; } }

        public Client(int balance, string Name, string password) : base(Name, password)
        {
            _balance = balance;
            _isBorrowing = false; 
            IsLibrarian = false;

        }
        public Client() : base()
        {

        }
        public override string ToString()
        {
            return "clinet " + base.ToString() + $" balance: {_balance} id: {_id}";
        }

    }

}
