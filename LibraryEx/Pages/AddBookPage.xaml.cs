using LibraryLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization.DateTimeFormatting;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryEx
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddBookPage : Page
    {
        LibrarySystem system;
        bool IsDateChange=false;
        Tuple<LibrarySystem, bool, LibraryItem> infoFromNavPage;

        public AddBookPage()
        {
            this.InitializeComponent();
            Application.Current.Suspending += Current_Suspending;
            publishDate_date.MaxYear = DateTimeOffset.Now.AddYears(0);
            string[] s = Enum.GetNames(typeof(Genres));
            for (int i = 0; i < s.Length; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                item.Content = s[i];
                genreCombox.Items.Add(item);
            }
            genreCombox.PlaceholderText = "Genres";
            FontWeight f = new FontWeight();
            f.Weight = 700;
            genreCombox.FontWeight = f;
            genreCombox.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            infoFromNavPage = (Tuple<LibrarySystem, bool, LibraryItem>)e.Parameter;
            system = infoFromNavPage.Item1;
           if(infoFromNavPage.Item2== true)
            {
                book_radioBtn.Visibility = Visibility.Collapsed;
                magazine_radioBtn_.Visibility = Visibility.Collapsed;
                IsDateChange = true;
                Name_txtBox.Text = infoFromNavPage.Item3.Name;
                publisher_txtBox.Text = infoFromNavPage.Item3.Publisher;
                price_txtBox.Text = infoFromNavPage.Item3.Price.ToString();
                id_txtBox.Text = "1111";
                id_txtBox.Visibility = Visibility.Collapsed;
                genreCombox.SelectedIndex = (int)infoFromNavPage.Item3.Genre;
                amount_txtBox.Text = infoFromNavPage.Item3.Amount.ToString();
                publishDate_date.Date = infoFromNavPage.Item3.DateOfPrinting;
                CreateBook_btn.Content = "Edit item";
                Book book = infoFromNavPage.Item3 as Book;
                if(book==null)
                {
                    Magazine magazine = infoFromNavPage.Item3 as Magazine;
                    magazine_radioBtn_.IsChecked = true;
                    edition_txtBox.Text = magazine.Edition.ToString();
                }
                else
                {
                    book_radioBtn.IsChecked = true;
                    auther_txtBox.Text = book.Auther;
                }


            }
        }

        private void magazine_radioBtn__Checked(object sender, RoutedEventArgs e)
        {
            edition_txtBox.Visibility = Visibility.Visible;
            auther_txtBox.Visibility = Visibility.Collapsed;
        }

        private void book_radioBtn_Checked(object sender, RoutedEventArgs e)
        {
            edition_txtBox.Visibility = Visibility.Collapsed;
            auther_txtBox.Visibility = Visibility.Visible;
        }

        private async void CreateBook_btn_Click(object sender, RoutedEventArgs e)
        {
            int checker;
            if(id_txtBox.Text==""|| amount_txtBox.Text == ""|| Name_txtBox.Text == ""||
                price_txtBox.Text == ""||publisher_txtBox.Text=="")
            {
                MessageDialog message = new MessageDialog("NOT ALL FIELDS WERE FILLED", "EROR!");
                await message.ShowAsync();
                return;
            } 
            if(book_radioBtn.IsChecked==false&&magazine_radioBtn_.IsChecked==false)
            {
                MessageDialog message = new MessageDialog("LIBRARY ITEM SPESIFICATION NEEDED", "EROR!");
                await message.ShowAsync();
                return;
            }
            if(book_radioBtn.IsChecked == true &&auther_txtBox.Text==""||
               magazine_radioBtn_.IsChecked == true && edition_txtBox.Text == "")
            {
                MessageDialog message = new MessageDialog("NOT ALL FIELDS WERE FILLED OR FILLED INCORRECT", "EROR!");
                await message.ShowAsync();
                return;
            } 
            if(genreCombox.SelectedIndex==-1)
            {
                MessageDialog message = new MessageDialog("GENRE WASNT SELECTED", "EROR!");
                await message.ShowAsync();
                return;
            }
            if (IsDateChange == false)
            {
                MessageDialog message = new MessageDialog("DATE OF PUBLISHING WASNT SELECTED", "EROR!");
                await message.ShowAsync();
                return;
            }
            if (infoFromNavPage.Item2 == true)
            {
                infoFromNavPage.Item3.Name = Name_txtBox.Text;
                infoFromNavPage.Item3.Amount = int.Parse(amount_txtBox.Text);
                infoFromNavPage.Item3.Price = int.Parse(price_txtBox.Text);
                infoFromNavPage.Item3.Publisher = publisher_txtBox.Text;
                infoFromNavPage.Item3.Genre = (Genres)genreCombox.SelectedIndex;
                infoFromNavPage.Item3.DateOfPrinting = publishDate_date.Date.DateTime;

                Book book = infoFromNavPage.Item3 as Book;
                if (book == null)
                {
                    Magazine magazine = infoFromNavPage.Item3 as Magazine;
                    magazine.Edition = int.Parse(edition_txtBox.Text);
                }
                else
                {
                    book.Auther = auther_txtBox.Text;
                }
                Frame.Navigate(typeof(ActionsPage), system);
                return;
            }
            if (id_txtBox.Text.Length!=4)
            {
                MessageDialog message = new MessageDialog("ID MUST BE COMPOSED OF 4 DIGITS", "EROR!");
                await message.ShowAsync();
                return;
            }
            checker = int.Parse(id_txtBox.Text);
            if (system.SearchById(checker).Count!=0)
            {
                MessageDialog message = new MessageDialog("THE ID YOU ENTERD IS ALLREADY IN USE", "EROR!");
                await message.ShowAsync();
                return;
            }
            if(book_radioBtn.IsChecked==true)
            {
                system.AddItem(new Book(auther_txtBox.Text, Name_txtBox.Text, publisher_txtBox.Text, (Genres)genreCombox.SelectedIndex, double.Parse(price_txtBox.Text), publishDate_date.Date.DateTime, checker,int.Parse(amount_txtBox.Text)));
            }
            else 
            {
                system.AddItem(new Magazine(int.Parse(edition_txtBox.Text), Name_txtBox.Text, publisher_txtBox.Text, (Genres)genreCombox.SelectedIndex, double.Parse(price_txtBox.Text), publishDate_date.Date.DateTime, checker, int.Parse(amount_txtBox.Text)));
            }
            genreCombox.Visibility = Visibility.Collapsed;
            genreCombox.SelectedIndex = -1;
            edition_txtBox.Visibility = Visibility.Collapsed;
            auther_txtBox.Visibility = Visibility.Collapsed;
            popup_canvas.Visibility = Visibility.Visible;
            amount_txtBox.Text = edition_txtBox.Text =
            auther_txtBox.Text = Name_txtBox.Text = publisher_txtBox.Text
            = price_txtBox.Text = id_txtBox.Text = "";
            book_radioBtn.IsChecked = magazine_radioBtn_.IsChecked = false;
            publishDate_date.Date = default;

        }

        private void popupYes_btn_Click(object sender, RoutedEventArgs e)
        {
            popup_canvas.Visibility = Visibility.Collapsed;
            genreCombox.Visibility = Visibility.Visible;

        }

        private void popupNo_btn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ActionsPage), system);
        }
        private void txtBoxNumb_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }
        private void txtBoxLetter_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => char.IsDigit(c));
        }
        private void txtBoxId_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
            if (args.NewText.Length > 4) args.Cancel = args.NewText.Any(c => c.GetType() == typeof(char));
        }
        private void publishDate_date_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            IsDateChange = true;
        }
        private void back_btn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ActionsPage), system);
        }
        private void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            system.SaveAll();
        }
    }
}
