﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Zlab.UWP.View.ViewModels;
// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Zlab.UWP.View.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>

    public sealed partial class AccountPage : Page
    {
        public AccountViewModel VM { get; set; } 
        public AccountPage()
        {
            this.InitializeComponent();
            this.VM = new AccountViewModel()
            {
                Email_Panel_Visibility = Visibility.Visible,
                Login_Panel_Visibility = Visibility.Collapsed,
                Signup_Panel_Visibility = Visibility.Collapsed
            }; 
        }
        

        private async void EmailBtn_Click(object sender, RoutedEventArgs e)
        {
            await VM.SendEmailCodeAsync();
        }

        private async void SignupBtn_Click(object sender, RoutedEventArgs e)
        {
            await VM.SignUpAsync();
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            await VM.LoginAsync(Dispatcher);
            Frame.Navigate(typeof(MessagePage));
        }

        private void LoginViewBtn_Click(object sender, RoutedEventArgs e)
        {
            VM.ShowLogin();
        }
    }
}
