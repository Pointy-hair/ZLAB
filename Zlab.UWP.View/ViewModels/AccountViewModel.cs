using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Zlab.UWP.View.Helpers;
using Zlab.UWP.View.Models;

namespace Zlab.UWP.View.ViewModels
{
    public class DispatcherManager
    {
        private CoreDispatcher _dispatcher;
        public CoreDispatcher Dispatcher
        {
            get
            {
                return _dispatcher;
            }
            set
            {
                _dispatcher = value;
            }
        }

        private static DispatcherManager _current;
        public static DispatcherManager Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new DispatcherManager();
                }
                return _current;
            }
        }
    }
    public class AccountViewModel : INotifyPropertyChanged
    {

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
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
                        DispatcherManager.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                           delegate ()
                           {
                               PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                           });
                    }
                }
            }
        }

        public void ShowLogin()
        {
            Email_Panel_Visibility = Visibility.Collapsed;
            Signup_Panel_Visibility = Visibility.Collapsed;
            Login_Panel_Visibility = Visibility.Visible;
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

        #endregion

        #region properties

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

        #endregion

        #region Functions
        public async Task SendEmailCodeAsync()
        {
            var sent = false;
            if (!String.IsNullOrEmpty(Email) && email.Contains("@") && email.Contains("."))
            {
                var result = await HttpHelper.GetAsync($"{Urls.SendEamil}?email={Email}");
                var obj = result.ToObj<ReturnResult<string>>();
                sent = obj.code == ReturnResult.SuccessCode;
            }
            if (sent)
            {
                Email_Panel_Visibility = Visibility.Collapsed;
                Signup_Panel_Visibility = Visibility.Visible;
            }

        }
        public async Task SignUpAsync()
        {
            var success = false;
            if (!string.IsNullOrEmpty(code))
            {
                var body = new SignupModel
                {
                    code = code,
                    email = email,
                    password = password,
                    username = username
                };
                var result = await HttpHelper.PostAsync(Urls.Signup, body.ToJson());
                var data = result.ToObj<ReturnResult<string>>();
                success = data.code == ReturnResult.SuccessCode;
                if (success)
                    UserName = data.data;
            }
            if (success)
            {
                Signup_Panel_Visibility = Visibility.Collapsed;
                Login_Panel_Visibility = Visibility.Visible;
            }
        }

        public async Task LoginAsync(CoreDispatcher dispatcher)
        {
            if (!string.IsNullOrEmpty(username))
            {
                var body = new SigninModel()
                {
                    username = username,
                    password = password
                };
                var result = await HttpHelper.PostAsync(Urls.Longin, body.ToJson());
                Debug.WriteLine(result);
                var data = result.ToObj<ReturnResult<UserTokenDto>>();
                if (data.code == ReturnResult.SuccessCode)
                {
                    var socketUrl = await HttpHelper.GetAsync($"{Urls.GetSocketUrl}?userid={data.data.userid}&token={data.data.token}");
                    Debug.WriteLine(socketUrl);
                    var urldata = socketUrl.ToObj<ReturnResult<string>>();
                    if(urldata.code == ReturnResult.SuccessCode)
                    {
                        await WebSocketService.Current.ConnectAsync(urldata.data, dispatcher);
                    }
                   
                   
                }

               
            }

        }
        #endregion
    }
}
