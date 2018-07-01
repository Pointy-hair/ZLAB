using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;

namespace Zlab.UWP.View.ViewModels
{
    public class AccountViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected async void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                if (DispatcherManager.Current.Dispatcher == null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
                else
                {
                    if (DispatcherManager.Current.Dispatcher.HasThreadAccess)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                    }
                    else
                    {
                        await DispatcherManager.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                            delegate ()
                            {
                                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                            });
                    }
                }
            }
        }
        #region visibilitys
        private Visibility email_panel_visibility;
        public Visibility Email_Panel_Visibility
        {
            get { return email_panel_visibility; }
            set { email_panel_visibility = value; OnPropertyChanged(); }
        }
        private Visibility signup_panel_visibility;
        public Visibility Signup_Panel_Visibility
        {
            get { return signup_panel_visibility; }
            set { signup_panel_visibility = value; OnPropertyChanged(); }
        }
        private Visibility login_panel_visibility;
        public Visibility Login_Panel_Visibility
        {
            get { return login_panel_visibility; }
            set { login_panel_visibility = value; OnPropertyChanged(); }
        }
        private string email;
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                email = value; OnPropertyChanged();
            }
        }
        private string username;
        public string UserName
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged();
            }
        }
        private string password;
        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }
        private string code;
        public string Code
        {
            get => code;
            set
            {
                code = value;
                OnPropertyChanged();
            }
        }
        #endregion
        public void SendEmailCodeAsync()
        {
            Email_Panel_Visibility = Visibility.Collapsed;
            Signup_Panel_Visibility = Visibility.Visible;
        }
    }
}
