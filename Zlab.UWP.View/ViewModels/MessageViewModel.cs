using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Zlab.UWP.View.Helpers;
using Zlab.UWP.View.Models;
using Zlab.UWP.View.Views;

namespace Zlab.UWP.View.ViewModels
{
    public class MessageViewModel : INotifyPropertyChanged
    {

        #region Properties

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
                        await DispatcherManager.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                           delegate ()
                           {
                               PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                           });
                    }
                }
            }
        }
        private ObservableCollection<Message> messages;
        public ObservableCollection<Message> Messages { get => messages; set { messages = value; OnPropertyChanged(); } }
        public MessageViewModel()
        {
            Messages = new ObservableCollection<Message>();
        }
        #endregion
        public void PageIn()
        {
            WebSocketService.Current.OnMessageReceived += Current_OnMessageReceivedAsync;
        }



        public  void PageOut()
        {
            WebSocketService.Current.OnMessageReceived -= Current_OnMessageReceivedAsync;
        }


        private async void Current_OnMessageReceivedAsync(object sender, MessageArgs args)
        {
            if (args.Messsage.type == PushType.MessageId)
            {
                var msgids = args.Messsage.msgids;
                var result = await HttpHelper.PostAsync(Urls.GetMessage, msgids.ToArray().ToJson());
                var data = result.ToObj<ReturnResult<List<Message>>>();
                foreach (var item in data.data)
                {
                    await MessagePage.Cuurent.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                     {
                         Messages.Add(item);
                     }); 
                }
                    
            }
        }
    }
}
