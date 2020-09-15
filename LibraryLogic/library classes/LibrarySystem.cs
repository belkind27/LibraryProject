using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LibraryLogic
{
    public class LibrarySystem
    {
        LibraryItemCollection _itemCollection;
        LibraryPersonCollections _personCollection;
        SaleCollection _saleCollection;
        List<Borrow> _totalBorrows;
        public List<Borrow> ClientsBorrows { get; set; }
        string _librarianRegPin;
        string pathDir;
        public List<Borrow> TotalBorrows { get { return _totalBorrows; } }
        public LibrarySystem(string pathDir)
        {
            this.pathDir = pathDir;
            LoadTotalBorrows(); 
            if(TotalBorrows!=null)
                foreach (Borrow item in TotalBorrows)
                {
                    item.Delay();
                }
            _itemCollection = new LibraryItemCollection(pathDir);
            _personCollection = new LibraryPersonCollections(pathDir);
            _saleCollection = new SaleCollection(pathDir);
            _librarianRegPin = "12345";

        }
        public int CurrentItemsInList { get { return _itemCollection.CurrentItemsInList; } }
        public int CurrentPersonInList { get { return _personCollection.CurrentPersonInList; } }
        public string Pin { get { return _librarianRegPin; } }
        public List<LibraryItem> ItemList { get { return _itemCollection.LibraryList; } }
        public List<Person> PersonList { get { return _personCollection.LibraryList; } }
        public List<Sale> SaleList { get { return _saleCollection.Sales; } }
        public  bool SalesCheck(LibraryItem item, out Double priceChange)
        {
            return _saleCollection.SalesCheck(item,out  priceChange);
        }
        public void AddSale(Sale sale)
        {
            try { _saleCollection.Add(sale); }
            catch (Exception e) { throw new LibrarySystemException(e.Message); }
        }
        public void AddItem(LibraryItem item)
        {
            try { _itemCollection.Add(item); }
            catch (Exception e)
            {
                throw new LibrarySystemException(e.Message);
            }
        }
        public void RemoveItem(LibraryItem item)
        {
            try { _itemCollection.Remove(item); }
            catch (Exception e)
            {
                throw new LibrarySystemException(e.Message);
            }
        }
        public void AddUser(Person person)
        {
            try { _personCollection.AddUser(person); }
            catch (Exception e) { throw new LibrarySystemException(e.Message); }
        }
        public void RemoveUser(Person person)
        {
            try { _personCollection.RemoveUser(person); }
            catch (Exception e)
            {
                throw new LibrarySystemException(e.Message);
            }
        }
        public Person FindUser(string Name, string password)
        {
            return _personCollection.FindUser(Name, password);
        }
        public List<LibraryItem> SearchByString(string searchString, int comboBoxIndex)
        {
            if (int.TryParse(searchString, out int i)) throw new LibrarySystemException("name cant be a number");
            if (comboBoxIndex == 0) return _itemCollection[searchString];
            if (comboBoxIndex == 1) return _itemCollection.SearchByAuther(searchString);
            if (comboBoxIndex == 2) return _itemCollection.SearchByPublisher(searchString);
            else return null;

        }
        public List<LibraryItem> SearchByGenre(int genreIndex)
        {
            return _itemCollection[(Genres)genreIndex];
        }
        public List<LibraryItem> SearchById(int Id)
        {
            return _itemCollection[Id];

        }
        private void SaveTotalBorrows()
        {
            Type[] Types = new Type[] { typeof(Book), typeof(Magazine) };
            XmlSerializer xmlSerializerperson = new XmlSerializer(typeof(List<Borrow>), Types);
            FileStream s = new FileStream($@"{pathDir}\borrowFile.xml", FileMode.Create);
            xmlSerializerperson.Serialize(s, _totalBorrows);
            s.Close();
        }
        private void LoadTotalBorrows()
        {
            Type[] Types = new Type[] { typeof(Book), typeof(Magazine) };
            XmlSerializer xmlSerializerItem = new XmlSerializer(typeof(List<Borrow>), Types);
            FileStream s;
           try { s = new FileStream($@"{pathDir}\borrowFile.xml", FileMode.Open); }
            catch (Exception)
            {
                s = new FileStream($@"{pathDir}\borrowFile.xml", FileMode.Create);
            }
            try { _totalBorrows = (List<Borrow>)xmlSerializerItem.Deserialize(s); }
            catch (Exception)
            {

            }
            finally { s.Close(); }
        }
        public Borrow Borrow(Client client,LibraryItem item, double SalePrice, bool isInSale=false)
        {
            if (item.Amount <= 0) throw new LibrarySystemException();
            if (client.IsBorrowing == true) throw new LibrarySystemException();
            item.Amount--;
            client.IsBorrowing = true;
            if (isInSale)
            {
                if (client.Balance - SalePrice <= 0) throw new LibrarySystemException();
                else client.Balance -= SalePrice;
            }
            else
            {
                if(client.Balance - item.Price<0) throw new LibrarySystemException();
                else client.Balance -= item.Price;
            }
            Borrow borrow = new Borrow(item, DateTime.Now,client.Id);
            _totalBorrows.Add(borrow);
            return borrow;
        }
        public void Return(Client client)
        {
            client.IsBorrowing = false;
            Borrow[] clientBorrows = TotalBorrows.FindAll((bo) => client.Id== bo.ClientsId).ToArray();
            clientBorrows[clientBorrows.Length - 1].IsBorrowActive = false;

        }
        public void SaveAll()
        {
            _itemCollection.Save();
            _personCollection.Save();
            _saleCollection.Save();
            SaveTotalBorrows();
        }
        public Client FindClientById(int id)
        {
            if (_personCollection.FindClientById(id) == null) throw new LibrarySystemException();
            return _personCollection.FindClientById(id);  
        }
         public void SetClientsBorrows(Client client)
        {
            if (TotalBorrows == null) return;
            ClientsBorrows = TotalBorrows.FindAll((bo) => client.Id == bo.ClientsId);
        }
         public void AddToclientsBorrows(Borrow borrow)
        {
            ClientsBorrows.Add(borrow);
        }

    }

}
