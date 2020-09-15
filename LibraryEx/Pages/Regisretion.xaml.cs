using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LibraryLogic;
using Windows.UI.Core.Preview;
using Windows.Storage;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace LibraryEx
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Regisretion : Page
    {
        LibrarySystem System;
        public Regisretion()
        {
            this.InitializeComponent();
            Application.Current.Suspending += Current_Suspending;
            password2_txtBox.KeyDown += Password2_txtBox_KeyDown;
            pin_txtBox.KeyDown += Pin_txtBox_KeyDown;
        }

        private void Pin_txtBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key.ToString() == "Enter") pin_btn_Click(pin_btn, new RoutedEventArgs());
        }

        private void Password2_txtBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key.ToString() == "Enter") CreateUser_btn_Click(CreateUser_btn, new RoutedEventArgs());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            System = (LibrarySystem)e.Parameter;
        }
        private async void pin_btn_Click(object sender, RoutedEventArgs e)
        {
            if(System.Pin!=pin_txtBox.Text)
            {
                MessageDialog message = new MessageDialog("YOUR PIN IS INCORRECT", "EROR!");
                await message.ShowAsync();
                return;
            }
            if (firstName_txtBox.Text.Length < 2 || lastName_txtBox.Text.Length < 2)
            {
                MessageDialog message = new MessageDialog("THE NAME YOU ENTERD IS INVALID", "EROR!");
                await message.ShowAsync();
                return;
            }

            if (password_txtBox.Text.Length > 10 || password_txtBox.Text.Length < 4)
            {
                MessageDialog message = new MessageDialog("YOUR PASSWORD MUST BE BETWEEN 4-10 CHARECTERS", "EROR!");
                await message.ShowAsync();
                return;
            }
            string fullName = $"{firstName_txtBox.Text} {lastName_txtBox.Text}";
            string password = password_txtBox.Text;
            Librarian librarian = new Librarian( fullName, password);
            try { System.AddUser(librarian); }
            catch (LibrarySystemException)
            {
                MessageDialog message = new MessageDialog("YOUR USER IS ALLREADY EXISTS,PLEASE LOGIN FROM THE LOGIN SCREEN", "EROR!");
                await message.ShowAsync();
                Frame.Navigate(typeof(MainPage));
            }
            UiMeneger.CurrentUser = librarian;
            Frame.Navigate(typeof(ActionsPage), System);
        }
        private async void CreateUser_btn_Click(object sender, RoutedEventArgs e)
        {
            if(librarrian_radioBtn.IsChecked == false&&client_radioBtn_.IsChecked==false)
            {
                MessageDialog message = new MessageDialog("YOU MUST DECLARE IF YOU ARE A LIBRARIAN OR A CLIENT", "EROR!");
                await message.ShowAsync();
                return;
            }
            else if(librarrian_radioBtn.IsChecked==true)
            {
                popup_canvas.Visibility = Visibility.Visible;
            }
            else 
            {
                if(password_txtBox.Text!=password2_txtBox.Text)
                {
                    MessageDialog message = new MessageDialog("THE PASSWORD CONFORMATION DOES NOT MATCH YOUR PASSWORD", "EROR!");
                    await message.ShowAsync();
                    return;
                }
                if(firstName_txtBox.Text.Length<2|| lastName_txtBox.Text.Length<2)
                {
                    MessageDialog message = new MessageDialog("THE NAME YOU ENTERD IS INVALID", "EROR!");
                    await message.ShowAsync();
                    return;
                }

                if (password_txtBox.Text.Length > 10 || password_txtBox.Text.Length < 4)
                {
                    MessageDialog message = new MessageDialog("YOUR PASSWORD MUST BE BETWEEN 4-10 CHARECTERS", "EROR!");
                    await message.ShowAsync();
                    return;
                }
                string fullName = $"{firstName_txtBox.Text} {lastName_txtBox.Text}";
                string password = password_txtBox.Text;
                Client client = new Client(0, fullName, password);
                try{ System.AddUser(client); }
                catch (LibrarySystemException)
                {
                    MessageDialog message = new MessageDialog("YOUR USER IS ALLREADY EXISTS,PLEASE LOGIN FROM THE LOGIN SCREEN", "EROR!");
                    await message.ShowAsync();
                    Frame.Navigate(typeof(MainPage));
                }
                UiMeneger.CurrentUser = client;
                Frame.Navigate(typeof(ActionsPage), System);
            }
        }

        private void exitPopup_btn1_Click(object sender, RoutedEventArgs e)
        {
            popup_canvas.Visibility = Visibility.Collapsed;
        }
        private void txtBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => char.IsDigit(c));
        }

        private void back_btn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), System);
        }
        private void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            System.SaveAll();
        }
    }
}
