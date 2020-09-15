using LibraryLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class AddSale : Page
    {
        LibrarySystem system;
        bool isDateChange ;
        Tuple<LibrarySystem, bool, Sale> infoFromNavPage;
        public AddSale()
        {
            this.InitializeComponent();
            Application.Current.Suspending += Current_Suspending;
            endDate_calendar.MinDate = DateTime.Now;
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
        private void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            system.SaveAll();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            infoFromNavPage = (Tuple < LibrarySystem, bool, Sale>)e.Parameter;
            system = infoFromNavPage.Item1;
            if (infoFromNavPage.Item2 == true)
            {
                GenreSale_radioBtn.Visibility = Visibility.Collapsed;
                otherSale_radioBtn_.Visibility = Visibility.Collapsed;
                isDateChange = true;
                Name_txtBox.Text = infoFromNavPage.Item3.Name;
                discount_comboBox.SelectedIndex = (infoFromNavPage.Item3.PrecentageOfDicrease) / 10 - 1;
                endDate_calendar.Date = infoFromNavPage.Item3.EndDate;
                CreateSale_btn.Content = "Edit sale";
                GenreSale sale = infoFromNavPage.Item3 as GenreSale;
                if (sale == null)
                {
                    StringSale sale1 = infoFromNavPage.Item3 as StringSale;
                    otherSale_radioBtn_.IsChecked = true;
                    search_combox.SelectedIndex = sale1.Filter;
                    word_txtBox.Text = sale1.Word;
                }
                else
                {
                    GenreSale_radioBtn.IsChecked = true;
                    genreCombox.SelectedIndex = (int)sale.Genre;
                }


            }
        }
        private void otherSale_radioBtn__Checked(object sender, RoutedEventArgs e)
        {
            genreCombox.Visibility = Visibility.Collapsed;
            word_txtBox.Visibility = Visibility.Visible;
            search_combox.Visibility = Visibility.Visible;
        }

        private void GenreSale_radioBtn_Checked(object sender, RoutedEventArgs e)
        {
            genreCombox.Visibility = Visibility.Visible;
            word_txtBox.Visibility = Visibility.Collapsed;
            search_combox.Visibility = Visibility.Collapsed;
        }
        private void search_combox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (search_combox.SelectedIndex == 0) word_txtBox.BeforeTextChanging -= Word_txtBox_BeforeTextChanging;
            else word_txtBox.BeforeTextChanging += Word_txtBox_BeforeTextChanging;
             word_txtBox.Text = "";
            ComboBoxItem comboBoxItem = search_combox.SelectedItem as ComboBoxItem;
            word_txtBox.PlaceholderText = (string)comboBoxItem.Content;   
        }

        private void Word_txtBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => char.IsDigit(c));
        }

        private async void CreateSale_btn_Click(object sender, RoutedEventArgs e)
        {
            if(Name_txtBox.Text==""||discount_comboBox.SelectedIndex==-1)
            {
                MessageDialog message = new MessageDialog("YOU MUST FILL ALL FIELDS", "EROR!");
                await message.ShowAsync();
                return;
            }
            if(GenreSale_radioBtn.IsChecked==false&&otherSale_radioBtn_.IsChecked==false)
            {
                MessageDialog message = new MessageDialog("YOU MUST FILL ALL FIELDS", "EROR!");
                await message.ShowAsync();
                return;
            }
            if(GenreSale_radioBtn.IsChecked == true &&genreCombox.SelectedIndex==-1|| otherSale_radioBtn_.IsChecked == true &&word_txtBox.Text=="")
            {
                MessageDialog message = new MessageDialog("YOU MUST FILL ALL FIELDS", "EROR!");
                await message.ShowAsync();
                return;
            }
            if(!isDateChange)
            {
                MessageDialog message = new MessageDialog("YOU MUST SELECT A DATE", "EROR!");
                await message.ShowAsync();
                return;
            }
            if (infoFromNavPage.Item2 == true)
            {

                infoFromNavPage.Item3.Name = Name_txtBox.Text;
                infoFromNavPage.Item3.PrecentageOfDicrease = (discount_comboBox.SelectedIndex + 1) * 10;
                DateTimeOffset timeOffset = (DateTimeOffset)endDate_calendar.Date;
                infoFromNavPage.Item3.EndDate = timeOffset.DateTime;
                GenreSale sale = infoFromNavPage.Item3 as GenreSale;
                if (sale == null)
                {
                    StringSale sale1 = infoFromNavPage.Item3 as StringSale;
                    sale1.Filter= search_combox.SelectedIndex;
                    sale1.Word= word_txtBox.Text;
                }
                else
                {
                   sale.Genre = (Genres)genreCombox.SelectedIndex;
                }
                Frame.Navigate(typeof(ActionsPage), system);
                return;
            }
            DateTimeOffset endDate = (DateTimeOffset)endDate_calendar.Date;
            if (otherSale_radioBtn_.IsChecked==true)
            {
                system.AddSale(new StringSale(word_txtBox.Text, search_combox.SelectedIndex,
                    Name_txtBox.Text, (discount_comboBox.SelectedIndex + 1) * 10, endDate.DateTime));
            } 
            else
            {
                system.AddSale(new GenreSale((Genres)genreCombox.SelectedIndex,
                 Name_txtBox.Text, (discount_comboBox.SelectedIndex + 1) * 10, endDate.DateTime));
            }
            genreCombox.Visibility = Visibility.Collapsed;
            popup_canvas.Visibility = Visibility.Visible;
            Name_txtBox.Text = "";
            word_txtBox.Text = "";
            genreCombox.SelectedIndex = -1;
            search_combox.SelectedIndex = 0;
            discount_comboBox.SelectedIndex = -1;
            GenreSale_radioBtn.IsChecked = false;
            otherSale_radioBtn_.IsChecked = false;
            endDate_calendar.Date = DateTime.Now;
        }


        private void endDate_calendar_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            isDateChange = true;
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
        private void back_btn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ActionsPage), system);
        }
    }
}
