using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace LibraryLogic
{
    public class LibraryPersonCollections
    {
        List<Person> _libraryList;
        int _currentPersonInList;
        int _idTogive;
        string pathDir;
        public int CurrentPersonInList { get {return _currentPersonInList; } }
        public LibraryPersonCollections(string pathDir)
        {
            this.pathDir = pathDir;
            Load();
            if (_libraryList == null) { _libraryList = new List<Person>();_idTogive = 1; }
            else
            {
                Person[] clients = _libraryList.FindAll((p) => p.GetType() == typeof(Client)).ToArray();
                if (clients.Length == 0) _idTogive = 1;
                else
                {
                    Client client = (Client)clients[clients.Length - 1];
                    _idTogive = client.Id + 1;
                }
            }
            _currentPersonInList = _libraryList.Count;
        }
        public List<Person> LibraryList
        { get { return _libraryList; } }
        public void AddUser(Person user)
        {

            if (FindUser(user.Name, user.Password) == null)
            {
                if(user is Client)
                {
                    Client client = (Client)user;
                    client.Id = _idTogive;
                    _idTogive++;
                }
                _libraryList.Add(user);
                _currentPersonInList++;
                return;
            }
           
            else throw new LibrarySystemException();
        }
        public void RemoveUser(Person user)
        {
            if (FindUser(user.Name, user.Password) == null) throw new LibrarySystemException();
            else
            {
                _libraryList.Remove(user);
                _currentPersonInList--;
                return;
            }


        }
        public Person FindUser(string Name, string password)
        {
            if (_libraryList == null)
            {
                _libraryList = new List<Person>();
                return null;
            }
            List<Person> FullNameList = _libraryList.FindAll(user => user.Name == Name);
            if (FullNameList.Count == 0) return null;
            Person p = FullNameList.Find(user => user.Password == password);
            return p;
        }
        public Client FindClientById(int id)
        {
            if (_libraryList == null)
            {
                _libraryList = new List<Person>();
                return null;
            }
            return (Client)_libraryList.Find(func);
            bool func(Person p)
            {
                if (p is Librarian) return false;
                else
                {
                    Client c = (Client)p;
                    return c.Id == id;
                }
            }
        }
        public void Save()
        {

            Type[] personTypes = new Type[] { typeof(Librarian), typeof(Client), typeof(Book), typeof(Magazine) };
            XmlSerializer xmlSerializerperson = new XmlSerializer(typeof(List<Person>), personTypes);
            FileStream s = new FileStream($@"{pathDir}\personFile.xml", FileMode.Create);
            xmlSerializerperson.Serialize(s, _libraryList);
            s.Close();

        }
        public void Load()
        {
            bool isFailed = false;
            Type[] personTypes = new Type[] { typeof(Librarian), typeof(Client), typeof(Book), typeof(Magazine) };
            XmlSerializer xmlSerializerItem = new XmlSerializer(typeof(List<Person>), personTypes);
            FileStream s;
            try { s = new FileStream($@"{pathDir}\personFile.xml", FileMode.Open); }
            catch (Exception)
            {
                isFailed = true;
                return;
                s = new FileStream($@"{pathDir}\personFile.xml", FileMode.Create);
            }
            if (!isFailed) { _libraryList = (List<Person>)xmlSerializerItem.Deserialize(s); s.Close(); }
            
        }
    }

}
