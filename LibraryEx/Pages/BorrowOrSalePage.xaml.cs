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
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryEx
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BorrowOrSalePage : Page
    {
        LibrarySystem system;
        Tuple<LibrarySystem, bool> infoFromNavPage;
        Sale selectedSale;
        delegate void TextBlockTapedEventHandler(object sender, TextBlockTappedEventArgs e);
        public BorrowOrSalePage()
        {
            this.InitializeComponent();
            Application.Current.Suspending += Current_Suspending;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            infoFromNavPage = (Tuple<LibrarySystem, bool>)e.Parameter;
            system = infoFromNavPage.Item1;
            if (infoFromNavPage.Item2)
            {
                Client client = UiMeneger.CurrentUser as Client;
                Borrow[] borrows = system.ClientsBorrows.ToArray();
                GridGenerateor<Borrow>(borrows);
            } 
            else
            {
                Sale[] sales = system.SaleList.ToArray();
                GridGenerateor<Sale>(sales);
            }

        }
        public async void GridGenerateor<T>(T[] array) where T:IknowDateDeadLine
        {
            if (array.Length == 0)
            {
                MessageDialog message = new MessageDialog("THERE IS NO ITEMS THAT MATCH YOUR SEARCH", "EROR!");
                await message.ShowAsync();
                Frame.Navigate(typeof(ActionsPage), system);
            }
            grid.Width = 800;
            grid.Height = array.Length * 200;
            for (int i = 0; i < array.Length; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(200);
                grid.RowDefinitions.Add(rowDefinition);
                ItemInGrid<T> item = new ItemInGrid<T>(array[i], grid, i,infoFromNavPage.Item2);
                item.textBlockTaped += Item_textBlockTaped;
            }

        }
        private void Item_textBlockTaped(object sender, TextBlockTappedEventArgs e)
        {
            confermtion_canvas.Visibility = Visibility.Visible;
            selectedSale = e.sale;
        }
        class ItemInGrid<T> where T : IknowDateDeadLine
        {
            //the page sign to the deligates
            public event TextBlockTapedEventHandler textBlockTaped;
            //items in grid
            Rectangle rectangle;
            TextBlock textBlock;
            //the item
            T Item;
            public ItemInGrid(T item, Grid grid, int rowCounter,bool isSale)
            {

                Item = item;
                rectangle = new Rectangle();
                rectangle.Width = 780;
                rectangle.Height = 180;
                if (!item.IsActive()) rectangle.Fill = new SolidColorBrush(Colors.OrangeRed);
                else rectangle.Fill = new SolidColorBrush(Colors.Green);
                textBlock = new TextBlock();
                textBlock.Width = 780;
                textBlock.Height = 180;
                textBlock.Text = item.ToString();
                textBlock.TextAlignment = TextAlignment.Center;
                FontWeight f = new FontWeight();
                f.Weight = 700;
                textBlock.FontWeight = f;
                textBlock.FontSize = 30;
                textBlock.TextWrapping = TextWrapping.WrapWholeWords;
                if (!isSale) textBlock.Tapped += TextBlock_Tapped;
                grid.Children.Add(rectangle);
                grid.Children.Add(textBlock);
                Grid.SetRow(rectangle, rowCounter);
                Grid.SetRow(textBlock, rowCounter);
            }
            private void TextBlock_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
            {
                Sale sale = Item as Sale;
                if (sale == null) return;
                else textBlockTaped.Invoke(this, new TextBlockTappedEventArgs(sale));
            }
        }
        class TextBlockTappedEventArgs : EventArgs
        {
            public Sale sale { get; set; }
            public TextBlockTappedEventArgs(Sale sale)
            {
                this.sale = sale;
            }
        }
        private void confermtionNo_btn_Click(object sender, RoutedEventArgs e)
        {
            confermtion_canvas.Visibility = Visibility.Collapsed;
        }
        private void confermtionYes_btn_Click(object sender, RoutedEventArgs e)
        {
            Tuple<LibrarySystem, bool,Sale> t = new Tuple<LibrarySystem, bool, Sale>(system, true, selectedSale);
            Frame.Navigate(typeof(AddSale), t);
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
