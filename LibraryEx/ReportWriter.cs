using LibraryLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace LibraryEx
{
    class ReportWriter
    {
         async public void ReportWrite(int indexSelected1, int indexSelected2, LibrarySystem system, DateTime dateTime1 = default, DateTime dateTime2 = default)
        {
            string[] fileNames = new string[] { $"borrowDelay", $"inventory", $"Users", "sales", "borrows" };
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFolder repoFolder;
            try { repoFolder = await folder.GetFolderAsync("Report Folder"); }
            catch (FileNotFoundException)
            {
                repoFolder = await folder.CreateFolderAsync("Report Folder");
            }
            StorageFile file;
            switch (indexSelected1)
            {
                case 0:
                    {
                        file = await repoFolder.CreateFileAsync(fileNames[indexSelected1] + DateTxtAdapt());
                        StringBuilder builder = new StringBuilder();
                        builder.AppendLine("clinet borrows that late:");
                        builder.AppendLine("****************************");
                        Borrow[] borrows = system.TotalBorrows.ToArray();
                        int counter = 0;
                        Client client;
                        for (int i = 0; i < borrows.Length; i++)
                        {
                            if (borrows[i].IsLate)
                            {
                                counter++;
                                builder.AppendLine(borrows[i].ReportView());
                                builder.AppendLine(system.FindClientById(borrows[i].ClientsId).ToString());
                                builder.AppendLine("****************************");

                            }
                        }
                        builder.AppendLine($"number of borrows late: {counter}");
                        await FileIO.WriteTextAsync(file, builder.ToString());
                        break;
                    }
                case 1:
                    {
                        file = await repoFolder.CreateFileAsync(fileNames[indexSelected1] + DateTxtAdapt());
                        StringBuilder builder = new StringBuilder();
                        int bookCounter = 0, mamgazineCounter = 0;
                        LibraryItem[] items = system.ItemList.ToArray();
                        builder.AppendLine("items in the system:");
                        builder.AppendLine("****************************");
                        for (int i = 0; i < items.Length; i++)
                        {
                            builder.AppendLine(items[i].ToString());
                            builder.AppendLine("****************************");
                            if (items[i] is Book) bookCounter++;
                            else mamgazineCounter++;

                        }
                        builder.AppendLine($"items in the system: {items.Length}");
                        builder.AppendLine($"books in the system: {bookCounter}");
                        builder.AppendLine($"magazines in the system: {mamgazineCounter}");
                        await FileIO.WriteTextAsync(file, builder.ToString());
                        break;
                    }
                case 2:
                    {
                        file = await repoFolder.CreateFileAsync(fileNames[indexSelected1] + DateTxtAdapt());
                        await FileIO.WriteTextAsync(file, WriteAll("users", system.PersonList.ToArray()).ToString());
                        break;

                    }
                case 3:
                    {
                        file = await repoFolder.CreateFileAsync(fileNames[indexSelected1] + DateTxtAdapt());
                        await FileIO.WriteTextAsync(file, WriteAll("sales", system.ItemList.ToArray()).ToString());
                        break;

                    }
                case 4:
                    {
                        if (indexSelected2 == 1)
                        {
                            StringBuilder builder = new StringBuilder();
                            string s;
                            if (dateTime2 == default)
                            {
                                s = $"{dateTime1.Year}.{dateTime1.Month}.{dateTime1.Day}To" + DateTxtAdapt();
                                builder.AppendLine($"borows in the system between {dateTime1.ToShortDateString()} to {DateTime.Today}");
                                dateTime2 = DateTime.Now;
                            }
                            else
                            {
                                s = $"{dateTime1.Year}.{dateTime1.Month}.{dateTime1.Day}To{dateTime2.Year}.{dateTime2.Month}.{dateTime2.Day}.txt";
                                builder.AppendLine($"borows in the system between {dateTime1.ToShortDateString()} to {DateTime.Today}");
                            }
                            file = await repoFolder.CreateFileAsync(fileNames[indexSelected1] + s);
                            builder.AppendLine("****************************");
                            int counter = 0;
                            double moneyCounter = 0;
                            var x = system.TotalBorrows.Where<Borrow>((B) => B.WhenBorrowHappend.CompareTo(dateTime1) >= 0 && B.WhenBorrowHappend.CompareTo(dateTime2) <= 0).ToList<Borrow>();
                            foreach (var item in x)
                            {
                                builder.AppendLine(item.ReportView());
                                builder.AppendLine("****************************");
                                moneyCounter += item.Price;
                                counter++;
                            }
                            builder.AppendLine($"borrows total: {counter} total revenu: {moneyCounter}");
                            await FileIO.WriteTextAsync(file, builder.ToString());
                            break;
                        }
                        else if (indexSelected2 == 2)
                        {
                            file = await repoFolder.CreateFileAsync(fileNames[indexSelected1] + DateTxtAdapt());
                            StringBuilder builder = new StringBuilder();
                            builder.AppendLine("active borrows in the system:");
                            builder.AppendLine("****************************");
                            Borrow[] borrows = system.TotalBorrows.ToArray();
                            double moneyCounter = 0;
                            int counter = 0;
                            for (int i = 0; i < borrows.Length; i++)
                            {
                                if (borrows[i].IsBorrowActive)
                                {
                                    builder.AppendLine(borrows[i].ReportView());
                                    builder.AppendLine("****************************");
                                    moneyCounter += borrows[i].Price;
                                    counter++;
                                }
                            }
                            builder.AppendLine($" active borrows in the system: {counter} total revenu: {moneyCounter}");
                            await FileIO.WriteTextAsync(file, builder.ToString());
                            break;
                        }
                        else
                        {
                            file = await repoFolder.CreateFileAsync(fileNames[indexSelected1] + DateTxtAdapt());
                            StringBuilder builder = new StringBuilder();
                            builder.AppendLine("borrows in the system:");
                            builder.AppendLine("****************************");
                            Borrow[] borrows = system.TotalBorrows.ToArray();
                            double moneyCounter = 0;
                            for (int i = 0; i < borrows.Length; i++)
                            {
                                builder.AppendLine(borrows[i].ReportView());
                                builder.AppendLine("****************************");
                                moneyCounter += borrows[i].Price;
                            }
                            builder.AppendLine($"borrows in the system: {borrows.Length} total revenu: {moneyCounter}");
                            await FileIO.WriteTextAsync(file, builder.ToString());
                            break;
                        }
                    }
            }
            StringBuilder WriteAll<T>(string item, T[] array)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(item + " in the system:");
                builder.AppendLine("****************************");
                for (int i = 0; i < array.Length; i++)
                {
                    builder.AppendLine(array[i].ToString());
                    builder.AppendLine("****************************");
                }
                builder.AppendLine(item + $" in the system: {array.Length}");
                return builder;
            }

        }
         private string DateTxtAdapt()
        {
            DateTime date = DateTime.Now;
            return $"{date.Year}.{date.Month}.{date.Day}.txt";
        }
    }
}
