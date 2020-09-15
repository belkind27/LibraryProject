using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;



namespace LibraryLogic
{
    public class LibraryItemCollection
    {
        List<LibraryItem> _libraryList;
        int _currentItemInList;
        string pathDir;
        public int CurrentItemsInList { get { return _currentItemInList; } }
        public LibraryItemCollection(string pathDir)
        {
            this.pathDir = pathDir;
            Load();
            if ( _libraryList == null) _libraryList = new List<LibraryItem>();
            _currentItemInList = _libraryList.Count;

        }
        public List<LibraryItem> LibraryList
        { get { return _libraryList; } }
        public void Add(LibraryItem item)
        {
            if (_libraryList == null)
            {
                _libraryList = new List<LibraryItem>();
            }
            if (_libraryList.Contains(item)) throw new LibrarySystemException();
            else
            {
                _currentItemInList++;
                _libraryList.Add(item);
               
            }
        }
        public void Remove(LibraryItem item)
        {
            if (!_libraryList.Contains(item)) throw new LibrarySystemException();
            else
            {
                _currentItemInList--;
                _libraryList.Remove(item);
                

            }
        }
        public List<LibraryItem> this[int id]
        {
            get
            {
                return _libraryList.FindAll((item) => item.Id == id);
            }
        }
        public List<LibraryItem> this[string name]
        {
            get
            {
                List<LibraryItem> list = _libraryList.FindAll((item) => item.Name.Contains(name));
                return list;
            }
        }
        public List<LibraryItem> this[Genres genre]
        {
            get
            {
                List<LibraryItem> list = _libraryList.FindAll((item) => item.Genre == genre);
                return list;
            }
        }
        public List<LibraryItem> SearchByPublisher(string publisher)
        {
            List<LibraryItem> list = _libraryList.FindAll((item) => item.Publisher == publisher);
            return list;
        }
        public List<LibraryItem> SearchByAuther(string Auther)
        {
            List<LibraryItem> list = _libraryList.FindAll((item) =>
            {
                if (item is Book)
                {
                    Book book = item as Book;
                    return book.Auther == Auther;
                }
                else { return false; }
                
            });
            return list;


        }
        public void Save()
        {

            Type[] itemTypes = new Type[] { typeof(Book), typeof(Magazine) };
            XmlSerializer xmlSerializerItem = new XmlSerializer(typeof(List<LibraryItem>), itemTypes);
            FileStream s = new FileStream($@"{pathDir}\itemFile.xml", FileMode.Create);
            xmlSerializerItem.Serialize(s, _libraryList);
            s.Close();

        }
        public void Load()
        { bool isFailed = false;
            Type[] itemTypes = new Type[] { typeof(Book), typeof(Magazine) };
            XmlSerializer xmlSerializerItem = new XmlSerializer(typeof(List<LibraryItem>), itemTypes);
            FileStream s;
           try { s = new FileStream($@"{pathDir}\itemFile.xml", FileMode.Open); }
            catch (Exception) 
            {
                isFailed = true;
                return;
                s = new FileStream($@"{pathDir}\itemFile.xml", FileMode.Create);
            }
            if(!isFailed) { _libraryList = (List<LibraryItem>)xmlSerializerItem.Deserialize(s); s.Close(); }

        }

    }
}
