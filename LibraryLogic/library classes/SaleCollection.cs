using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace LibraryLogic
{
    public class SaleCollection
    {
        List<Sale> _sales;
        int _currentNumberOfSales;
        string pathDir;
        public SaleCollection(string pathDir)
        {
            this.pathDir = pathDir;
            Load();
            if (_sales == null) _sales = new List<Sale>();
            _currentNumberOfSales = _sales.Count;
        }
        public List<Sale> Sales { get { return _sales; } }
        public void Add(Sale sale)
        {

            if (_sales.Contains(sale)) throw new LibrarySystemException();
            else
            {
                _currentNumberOfSales++;
                _sales.Add(sale);
            }
        }
        public bool SalesCheck(LibraryItem item, out Double priceChange)
        {
            Sale[] sales = _sales.ToArray();
            double minNumber = 10000000;
            priceChange = default;
            for (int i = 0; i < sales.Length; i++)
            {
                if (sales[i].itemOnSaleCheck(item) < minNumber)
                    minNumber = sales[i].itemOnSaleCheck(item);
            }
            if (minNumber == 10000000) return false;
            else
            {
                priceChange = minNumber;
                return true;
            }


        }
        public void Update()
        {
            if (_sales == null) return;
            Sale[] arr = _sales.ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                if (!arr[i].IsActive())
                {
                    _sales.Remove(arr[i]);
                    _currentNumberOfSales--;
                }
            }
        }
        public void Save()
        {
            Update();
            Type[] itemTypes = new Type[] { typeof(StringSale), typeof(GenreSale) };
            XmlSerializer xmlSerializerItem = new XmlSerializer(typeof(List<Sale>), itemTypes);
            FileStream s = new FileStream($@"{pathDir}\SaleFile.xml", FileMode.Create);
            xmlSerializerItem.Serialize(s, _sales);
            s.Close();

        }
        public void Load()
        {
            bool isExeption = false;
            Type[] itemTypes = new Type[] { typeof(StringSale), typeof(GenreSale) };
            XmlSerializer xmlSerializerItem = new XmlSerializer(typeof(List<Sale>), itemTypes);
            FileStream s;
            try { s = new FileStream($@"{pathDir}\SaleFile.xml", FileMode.Open); }
            catch (Exception)
            {
                isExeption = true;
                return;
                s = new FileStream($@"{pathDir}\SaleFile.xml", FileMode.Create);
            }
            if (!isExeption) { _sales = (List<Sale>)xmlSerializerItem.Deserialize(s);
                s.Close(); if (!isExeption) Update(); }

        }

    }

}
