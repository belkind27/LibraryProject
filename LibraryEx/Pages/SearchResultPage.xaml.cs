using LibraryLogic;
using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryEx
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchResultPage : Page
    {
        LibrarySystem system;
        Tuple<LibrarySystem, List<LibraryItem>, int> infoFromNavPage;
        LibraryItem itemSelected;
        bool isItemOnSale;
        bool isItemOnSaleSelected = false;
        string action;
        delegate void TextBlockHoverEventHandler(object sender, TextBlockHoverEventArgs e);
        delegate void TextBlockHoverEndsEventHandler();
        delegate void TextBlockTapedEventHandler(object sender, TextBlockHoverEventArgs e);
        public SearchResultPage()
        {
            this.InitializeComponent();
            Application.Current.Suspending += Current_Suspending;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            infoFromNavPage= (Tuple<LibrarySystem, List<LibraryItem>, int>)e.Parameter;
            system = infoFromNavPage.Item1;
            Refrash();
        }
        public async void Refrash()
        {
            LibraryItem[] searchResult = infoFromNavPage.Item2.ToArray();
            if (searchResult.Length == 0)
            {
                MessageDialog message = new MessageDialog("THERE IS NO ITEMS THAT MATCH YOUR SEARCH", "EROR!");
                await message.ShowAsync();
                Frame.Navigate(typeof(ActionsPage), system);
            }
            int numOfSearchResult = searchResult.Length;
            grid.Width = 800;
            int gridSize = 200;
            int maxInColoumn = 4;
            if (searchResult.Length <= 4)
            {
                gridSize = 400;
                maxInColoumn = 2;
            }
            int gridHeight = numOfSearchResult / maxInColoumn + 1;
            gridHeight *= gridSize;
            grid.Height = gridHeight;
            for (int i = 0; i < gridHeight / gridSize; i++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(gridSize);
                grid.RowDefinitions.Add(rowDefinition);
            }
            for (int i = 0; i < maxInColoumn; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(gridSize);
                grid.ColumnDefinitions.Add(columnDefinition);
            }
            int columnCounter = 0;
            int rowCounter = 0;
            gridSize = gridSize / 10 * 9;
            for (int i = 0; i < numOfSearchResult; i++)
            {

                ItemInGrid item = new ItemInGrid(searchResult[i], grid, columnCounter, rowCounter,gridSize);
                item.textBlockHover += Item_textBlockHover;
                item.textBlockHoverEnds += Item_textBlockHoverEnds;
                item.textBlockTaped += Item_textBlockTaped;

                columnCounter++;
                if (columnCounter == maxInColoumn) { columnCounter = 0; rowCounter++; }
            }

        }

        private void Item_textBlockTaped(object sender, TextBlockHoverEventArgs e)
        {
            if (isItemOnSale) isItemOnSaleSelected = true;
            confermtion_canvas.Visibility = Visibility.Visible;
            itemSelected = e.Item;
            switch (infoFromNavPage.Item3)
            {
                case 1: action = "borrow"; break;
                case 2: action = "edit"; break;
                case 3: action = "remove"; break;
                default: action = ""; break;
            }
            Book book = itemSelected as Book;
            if (book == null) { confermtion_txtblock.Text = $"are you sure you want to {action} this magazine?"; }
            else confermtion_txtblock.Text = $"are you sure you want to {action} this book?";
        }

        private void Item_textBlockHoverEnds()
        {
            detail_panel.Visibility = Visibility.Collapsed;
            isItemOnSale = false;
        }

        private void Item_textBlockHover(object sender, TextBlockHoverEventArgs e)
        {
            double SalePrice;
            if (system.SalesCheck(e.Item, out SalePrice))
            {
                isItemOnSale = true;
                price_txtblock.Text = $"Special Price: {SalePrice}";
                price_txtblock.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                price_txtblock.Text = $"Price: {e.Item.Price}";
                price_txtblock.Foreground = new SolidColorBrush(Colors.Black);
            }
            name_txtblock.Text = $"Name: {e.Item.Name} ";
            id_txtblock.Text = $"Id: {e.Item.Id} ";
            genre_txtblock.Text = $"Genre: {e.Item.Genre}";
            date_txtblock.Text = $"Date: {e.Item.DateOfPrinting.ToShortDateString()}";
            publisher_txtblock.Text = $"Publisher: {e.Item.Publisher}";
            Book book = e.Item as Book;
            if (book == null)
            {
                Magazine magazine = e.Item as Magazine;
                autherOrEdition_txtblock.Text = $"Edition: {magazine.Edition}";
            }
            else autherOrEdition_txtblock.Text = $"Auther: {book.Auther}";
            detail_panel.Visibility = Visibility.Visible;

        }

        class ItemInGrid
        {
            //the page sign to the deligates
            public event TextBlockHoverEventHandler textBlockHover;
            public event TextBlockHoverEndsEventHandler textBlockHoverEnds;
            public event TextBlockTapedEventHandler textBlockTaped;
            //items in grid
            Rectangle rectangle;
            TextBlock textBlock;
            //the item
            LibraryItem Item;

            public ItemInGrid(LibraryItem item, Grid grid, int columnCounter, int rowCounter,int size)
            {

                Item = item;
                rectangle = new Rectangle();
                rectangle.Width = size;
                rectangle.Height = size;
                rectangle.Fill = new SolidColorBrush(Colors.AliceBlue);
                textBlock = new TextBlock();
                textBlock.Width = size;
                textBlock.Height = size;
                textBlock.Text = item.Name;
                textBlock.TextAlignment = TextAlignment.Center;
                FontWeight f = new FontWeight();
                f.Weight = 700;
                textBlock.FontWeight = f;
                textBlock.FontSize = 20;
                textBlock.TextWrapping = TextWrapping.WrapWholeWords;
                textBlock.PointerEntered += TextBlock_PointerEntered;
                textBlock.PointerExited += TextBlock_PointerExited;
                textBlock.Tapped += TextBlock_Tapped;
                grid.Children.Add(rectangle);
                grid.Children.Add(textBlock);
                Grid.SetColumn(rectangle, columnCounter);
                Grid.SetRow(rectangle, rowCounter);
                Grid.SetColumn(textBlock, columnCounter);
                Grid.SetRow(textBlock, rowCounter);
            }


            private void TextBlock_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
            {
                textBlockTaped.Invoke(this, new TextBlockHoverEventArgs(Item));
            }

            private void TextBlock_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
            {
                textBlockHoverEnds.Invoke();
            }

            private void TextBlock_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
            {

                textBlockHover.Invoke(this, new TextBlockHoverEventArgs(Item));

            }
        }

        class TextBlockHoverEventArgs : EventArgs
        {
            public LibraryItem Item { get; set; }
            public TextBlockHoverEventArgs(LibraryItem item)
            {
                Item = item;
            }
        }

        private async void confermtionYes_btn_Click(object sender, RoutedEventArgs e)
        {
            double SalePrice=0;
            if (isItemOnSaleSelected)
            {
                system.SalesCheck(itemSelected, out SalePrice);
            }
            switch (infoFromNavPage.Item3)
            {
                case 1:
                    Client client = UiMeneger.CurrentUser as Client;
                    if (client.IsBorrowing)
                    {
                        MessageDialog message = new MessageDialog("A CLIENT CAN BORROW 1  ITEM AT A TIME", "EROR!");
                        await message.ShowAsync();
                        Frame.Navigate(typeof(ActionsPage), system);
                    }
                    else if (itemSelected.Amount == 0)
                    {
                        MessageDialog message = new MessageDialog("THE IITEM YOU SELECTED CURRENTLY OUT OF STOCK", "EROR!");
                        await message.ShowAsync();
                        confermtion_canvas.Visibility = Visibility.Collapsed;
                        return;
                    }
                    else if (client.Balance - itemSelected.Price < 0||
                        isItemOnSaleSelected&& client.Balance - SalePrice < 0)
                    {
                        MessageDialog message = new MessageDialog("You DONT HAVE ENOUGH MONEY", "EROR!");
                        await message.ShowAsync();
                        confermtion_canvas.Visibility = Visibility.Collapsed;
                        return;
                    }
                    else
                    {
                        system.AddToclientsBorrows(system.Borrow(client, itemSelected, SalePrice, isItemOnSaleSelected));
                        Frame.Navigate(typeof(ActionsPage), system);
                    }
                    break;
                case 2:
                    Tuple<LibrarySystem, bool, LibraryItem> t = new Tuple<LibrarySystem, bool, LibraryItem>(system, true, itemSelected);
                    Frame.Navigate(typeof(AddBookPage), t);
                    ; break;
                case 3:
                    if(system.TotalBorrows.Find((b => b.IsBorrowActive && b.Item.Id == itemSelected.Id))!=null)
                    {
                        MessageDialog message2 = new MessageDialog("YOU CANT REMOVE A BOOK WHILE ITS BORROWD","EROR");
                        await message2.ShowAsync();
                        return;
                    }
                    system.RemoveItem(itemSelected);
                    MessageDialog message1 = new MessageDialog($"YOU REMOVED {itemSelected.Name}");
                    await message1.ShowAsync();
                    confermtion_canvas.Visibility = Visibility.Collapsed;
                    grid.Children.Clear();
                    Refrash();
                    break;
                default: break;
            }
        }

        private void confermtionNo_btn_Click(object sender, RoutedEventArgs e)
        {
            confermtion_canvas.Visibility = Visibility.Collapsed;
            isItemOnSaleSelected = false;

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



