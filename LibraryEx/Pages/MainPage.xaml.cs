using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core.Preview;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LibraryLogic;
using Windows.Storage;
using System.ComponentModel;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LibraryEx
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        LibrarySystem system;

        public MainPage()
        {
            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = new Windows.Foundation.Size((double)1280, (double)720);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            system = new LibrarySystem(ApplicationData.Current.LocalFolder.Path);
            password_box.KeyDown += Password_box_KeyDown;
            Application.Current.Suspending += Current_Suspending;
        }

        private void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            system.SaveAll();
        }
        private  void  Password_box_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
             if (e.Key.ToString() == "Enter")login_btn_Click(new Object(), new RoutedEventArgs());                   
        }
        private void newUser_btn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Regisretion), system);
        }
        private async void login_btn_Click(object sender, RoutedEventArgs e)
        {

            string fullName = $"{firstName_txtBox.Text} {lastName_txtBox.Text}";
            string password = password_box.Password;
            UiMeneger.CurrentUser = system.FindUser(fullName, password);
            if (UiMeneger.CurrentUser == null)
            {
                firstName_txtBox.Text = "";
                lastName_txtBox.Text = "";
                password_box.Password = "";
                MessageDialog message = new MessageDialog("NAME OR PASSWORD INCORRECT", "EROR!");
                await message.ShowAsync();

            } 
            else
            {
                Frame.Navigate(typeof(ActionsPage), system);
            }

        }
        private void txtBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => char.IsDigit(c));
        }

    }
}
