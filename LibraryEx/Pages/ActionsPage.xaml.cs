using System;
using System.Linq;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LibraryLogic;
using Windows.UI.Text;
using System.Collections.Generic;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryEx
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ActionsPage : Page
    {
        LibrarySystem System;
        Client client;
        bool isReportView, isSearchView;
        ReportWriter ReportWriter;
        int actionFilter;
        int ComboBoxIndex;
        public ActionsPage()
        {
            this.InitializeComponent();
            search_txtbox.TextChanged += Search_txtbox_TextChanged;
            ReportWriter = new ReportWriter();
            Application.Current.Suspending += Current_Suspending;
            report_calender.MaxDate = DateTime.Now;
            report_calender.SelectionMode = CalendarViewSelectionMode.Multiple;
            search_txtbox.KeyDown += Search_txtbox_KeyDown;
            addBalance_txtbox.KeyDown += AddBalance_txtbox_KeyDown;
            string[] s = Enum.GetNames(typeof(Genres));
            for (int i = 0; i < s.Length; i++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                item.Content = s[i];
                genreCombox.Items.Add(item);
            }
            FontWeight f = new FontWeight();
            f.Weight = 700;
            genreCombox.FontWeight = f;
            genreCombox.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        }
        private void Search_txtbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            guessSearch_listView.Items.Clear();
           TextBox t = (TextBox)sender;
            if (t.Text == null || t.Text == ""|| ComboBoxIndex!=0) return;
            List<LibraryItem> list = System.SearchByString(t.Text, ComboBoxIndex);
            foreach (var item in list)
            {
                ListViewItem viewItem = new ListViewItem();
                viewItem.Content = item.Name;
                viewItem.Tapped += ViewItem_Tapped;
                guessSearch_listView.Items.Add(viewItem);
            }
        }

        private void ViewItem_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            ListViewItem item = (ListViewItem)sender;
            Tuple<LibrarySystem, List<LibraryItem>, int> t =
           new Tuple<LibrarySystem, List<LibraryItem>, int>
           (System, System.SearchByString(item.Content.ToString(), 0), actionFilter);
            Frame.Navigate(typeof(SearchResultPage), t);
        }

        private void AddBalance_txtbox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key.ToString() == "Enter") enterBalance_btn_Click(enterBalance_btn, new RoutedEventArgs());
        }

        private void Search_txtbox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key.ToString() == "Enter") search_btn_Click(search_btn, new RoutedEventArgs());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            System = (LibrarySystem)e.Parameter;
            //name of user
            bannar_txtblok.Text = UiMeneger.CurrentUser.Name;
            //user balance
            if (UiMeneger.CurrentUser.IsLibrarian == true)
            {
                librarian_canvas.Visibility = Visibility.Visible;
                balance_img.Visibility = Visibility.Collapsed;
                balance_txtblock.Visibility = Visibility.Collapsed;
                client_canvas.Visibility = Visibility.Collapsed;
                search_canvas.Visibility = Visibility.Collapsed;
            }
            else
            {
                client = (Client)UiMeneger.CurrentUser;
                System.SetClientsBorrows(client);
                if (System.SaleList.Count != 0)
                {
                    Sale[] sales = System.SaleList.ToArray();
                    for (int i = 0; i < sales.Length; i++)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Content = sales[i].ToString();
                        item.DataContext = sales[i];
                        item.Tapped += Item_Tapped;
                        salePopUp_listview.Items.Add(item);
                    }
                    salePopUp_canvas.Visibility = Visibility.Visible;
                }                
                balance_txtblock.Text = $"Balance:{client.Balance}";
                if (client.IsBorrowing == false) return_btn.IsEnabled = false;
                else return_btn.IsEnabled = true;
                librarian_canvas.Visibility = Visibility.Collapsed;
                balance_img.Visibility = Visibility.Visible;
                balance_txtblock.Visibility = Visibility.Visible;
                client_canvas.Visibility = Visibility.Visible;
                search_canvas.Visibility = Visibility.Visible;
                IfClientLate();
            }
        }

        private void Item_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Tuple<LibrarySystem, List<LibraryItem>, int> t;
            ListViewItem item = sender as ListViewItem;
            Sale sale = (Sale)item.DataContext;
            GenreSale genreSale = sale as GenreSale;
            if (genreSale == null)
            {
                StringSale stringSale = sale as StringSale;
                t = new Tuple<LibrarySystem, List<LibraryItem>, int>
                (System, System.SearchByString(stringSale.Word, stringSale.Filter), 1);
                Frame.Navigate(typeof(SearchResultPage), t);
            }
            else
            {
                t = new Tuple<LibrarySystem, List<LibraryItem>, int>
                (System, System.SearchByGenre((int)genreSale.Genre), 1);
                Frame.Navigate(typeof(SearchResultPage), t);
            }
        }

        private void search_combox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (search_combox.SelectedIndex == 4)
            {
                genreCombox.Visibility = Visibility.Visible;
                ComboBoxIndex = 4;
                search_txtbox.Text = "";
                search_txtbox.IsEnabled = false;

            }
            else
            {
               genreCombox.Visibility = Visibility.Collapsed;
                search_txtbox.Text = "";
                ComboBoxIndex = search_combox.SelectedIndex;
                search_txtbox.IsEnabled = true;
            }
        }

        private async void search_btn_Click(object sender, RoutedEventArgs e)
        {
            Tuple<LibrarySystem, List<LibraryItem>, int> t;
            if (UiMeneger.CurrentUser is Client)
            {
                actionFilter = 1;
            }
            if (ComboBoxIndex == 4)
            {
                int i = genreCombox.SelectedIndex;
                if (i < 0)
                {
                    MessageDialog message = new MessageDialog("CHOOSE GENRE", "EROR!");
                    await message.ShowAsync();
                    return;
                }
                t = new Tuple<LibrarySystem, List<LibraryItem>, int>(System, System.SearchByGenre(i), actionFilter);
                Frame.Navigate(typeof(SearchResultPage), t);
            }
            if (ComboBoxIndex == 3)
            {
                t = new Tuple<LibrarySystem, List<LibraryItem>, int>
               (System, System.SearchById(int.Parse(search_txtbox.Text)), actionFilter);
                Frame.Navigate(typeof(SearchResultPage), t);
            }
            else
            {
                t = new Tuple<LibrarySystem, List<LibraryItem>, int>
              (System, System.SearchByString(search_txtbox.Text, ComboBoxIndex), actionFilter);
                Frame.Navigate(typeof(SearchResultPage), t);
            }
        }

        private async void enterBalance_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(addBalance_txtbox.Text, out int tmp) || client.Balance + int.Parse(addBalance_txtbox.Text) > 999)
            {
                MessageDialog message = new MessageDialog("THE BALANCE YOU ENTERD IS INVALID", "EROR!");
                await message.ShowAsync();
                return;
            }
            else
            {
                client.Balance += double.Parse(addBalance_txtbox.Text);
                addBalance_txtbox.Text = "";
                balance_txtblock.Text = $"Balance:{client.Balance}";
                addBalance_canvas.Visibility = Visibility.Collapsed;
            }
        }

        private void addBalance_btn_Click(object sender, RoutedEventArgs e)
        {
            addBalance_canvas.Visibility = Visibility.Visible;

        }

        private void exitBalancePop_Click(object sender, RoutedEventArgs e)
        {
            addBalance_canvas.Visibility = Visibility.Collapsed;
        }

        private void removebook_btn_Click(object sender, RoutedEventArgs e)
        {
            actionFilter = 3;
            search_canvas.Visibility = Visibility.Visible;
            librarian_canvas.Visibility = Visibility.Collapsed;
            isSearchView = true;
        }

        private void editBook_btn_Click(object sender, RoutedEventArgs e)
        {
            actionFilter = 2;
            search_canvas.Visibility = Visibility.Visible;
            librarian_canvas.Visibility = Visibility.Collapsed;
            isSearchView = true;
        }

        private void addBook_btn_Click(object sender, RoutedEventArgs e)
        {
            Tuple<LibrarySystem, bool, LibraryItem> t = new Tuple<LibrarySystem, bool, LibraryItem>(System, false, null);
            Frame.Navigate(typeof(AddBookPage), t);

        }

        private void confermtionNo_btn_Click(object sender, RoutedEventArgs e)
        {
            confermtion_canvas.Visibility = Visibility.Collapsed;
        }

        private void confermtionYes_btn_Click(object sender, RoutedEventArgs e)
        {
            System.Return(client);
            confermtion_canvas.Visibility = Visibility.Collapsed;
            return_btn.IsEnabled = false;
        }

        private void return_btn_Click(object sender, RoutedEventArgs e)
        {
            System.ClientsBorrows.Last().IsBorrowActive = false;
            confermtion_canvas.Visibility = Visibility.Visible;
            confermtion_txtblock.Text = $"are you sure you want to return { System.ClientsBorrows.Last().Item.Name}";
        }
        private void txtBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (search_combox.SelectedIndex == 1 ||
            search_combox.SelectedIndex == 2) args.Cancel = args.NewText.Any(c => char.IsDigit(c));
            if (search_combox.SelectedIndex == 3)
            {
                args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
                if (args.NewText.Length > 4) args.Cancel = args.NewText.Any(c => c.GetType() == typeof(char));
            }
        }
        private void txtBoxBalance_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        private void borrow_btn_Click(object sender, RoutedEventArgs e)
        {
            Tuple<LibrarySystem, bool> t = new Tuple<LibrarySystem, bool>(System, true);
            Frame.Navigate(typeof(BorrowOrSalePage), t);
        }

        private async void IfClientLate()
        {
            if (System.ClientsBorrows == null || System.ClientsBorrows.Count == 0) return;
            if (System.ClientsBorrows.Last().IsLate)
            {
                MessageDialog message = new MessageDialog("YOU ARE LATE ON RETURNING YOUR LATEST BORROW", "EROR!");
                await message.ShowAsync();
                return;
            }
        }

        private void addSale_btn_Click(object sender, RoutedEventArgs e)
        {
            Tuple<LibrarySystem, bool, Sale> t = new Tuple<LibrarySystem, bool, Sale>(System, false, null);
            Frame.Navigate(typeof(AddSale), t);
        }

        private void editSale_btn_Click(object sender, RoutedEventArgs e)
        {
            Tuple<LibrarySystem, bool> t = new Tuple<LibrarySystem, bool>(System, false);
            Frame.Navigate(typeof(BorrowOrSalePage), t);
        }
        private void back_btn_Click(object sender, RoutedEventArgs e)
        {
            if (isReportView) 
            {
                report_combox.Visibility = Visibility.Collapsed;
                createReport_btn.Visibility = Visibility.Collapsed;
                reportOption_combox.Visibility = Visibility.Collapsed;
                report_calender.Visibility = Visibility.Collapsed;
                librarian_canvas.Visibility = Visibility.Visible;
                isReportView = false;
                return;
            }
            if(isSearchView)
            {
                search_canvas.Visibility = Visibility.Collapsed;
                librarian_canvas.Visibility = Visibility.Visible;
                isSearchView = false;
                return;
            }
            Frame.Navigate(typeof(MainPage), System);
        }

        private void report_btn_Click(object sender, RoutedEventArgs e)
        {
            librarian_canvas.Visibility = Visibility.Collapsed;
            report_combox.Visibility = Visibility.Visible;
            createReport_btn.Visibility = Visibility.Visible;
            isReportView = true;
        }

        private void report_combox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (reportOption_combox == null) return;
            if (report_combox.SelectedIndex == 4)
            {
                reportOption_combox.Visibility = Visibility.Visible;
                Canvas.SetLeft(createReport_btn, 679);
            }
            else
            {
                reportOption_combox.Visibility = Visibility.Collapsed;
                Canvas.SetLeft(createReport_btn, 442);
            }
        }
        private async void report_btn1_Click(object sender, RoutedEventArgs e)
        {
            if (report_combox.SelectedIndex == 4)
            {
                if(reportOption_combox.SelectedIndex==1)
                {
                    DateTimeOffset[] dates = report_calender.SelectedDates.ToArray();
                    if (dates.Length == 0 || dates.Length > 2)
                    {
                        MessageDialog message1 = new MessageDialog("YOU NEED TO CHOOSE 1 OR 2 DATES", "EROR!");
                        await message1.ShowAsync();
                        return;
                    }
                    if (dates.Length == 1)
                    {
                        ReportWriter.ReportWrite(report_combox.SelectedIndex, reportOption_combox.SelectedIndex, System, dates[0].DateTime);
                    }
                    if (dates.Length == 2)
                    {
                        DateTime d1 = dates[0].DateTime, d2 = dates[1].DateTime;
                        if (d1 < d2) ReportWriter.ReportWrite(report_combox.SelectedIndex, reportOption_combox.SelectedIndex, System, d1, d2);
                        else ReportWriter.ReportWrite(report_combox.SelectedIndex, reportOption_combox.SelectedIndex, System, d2, d1);
                    }
                }
                else ReportWriter.ReportWrite(report_combox.SelectedIndex, reportOption_combox.SelectedIndex, System);

            }
            else
            {
                ReportWriter.ReportWrite(report_combox.SelectedIndex, reportOption_combox.SelectedIndex, System);
            }
            MessageDialog message = new MessageDialog("THE REPORT WAS CREATED");
            await message.ShowAsync();
            return;
        }
        private void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            System.SaveAll();
        }

        private void exitSaleePop_Click(object sender, RoutedEventArgs e)
        {
            salePopUp_canvas.Visibility = Visibility.Collapsed;
        }

        private void reportOption_combox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (reportOption_combox.SelectedIndex == 1)
            {
                Canvas.SetLeft(createReport_btn, 1009);
                report_calender.Visibility = Visibility.Visible;
            }
            else
            {
                report_calender.Visibility = Visibility.Collapsed;
                Canvas.SetLeft(createReport_btn, 679);
            }
        }
    }
}
