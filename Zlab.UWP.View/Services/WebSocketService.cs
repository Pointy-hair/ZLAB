using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Core;
using Zlab.UWP.View.Helpers;
using Zlab.UWP.View.Models;

namespace Zlab.UWP.View
{
    public class WebSocketService
    {

        public delegate void OnMessageHanlder(object sender, MessageArgs args);
        public event OnMessageHanlder OnMessageReceived;
        public static WebSocketService Current { get; } = new WebSocketService();
        private HubConnection connection;
        private readonly static string HeartBeat = "::";
        private readonly static string HeartBeatMethodName = "HeartBeat";
        public async Task ConnectAsync(string url, CoreDispatcher dispatcher)
        {


            connection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();
            connection.Closed += Connection_Closed;


            try
            {
                await connection.StartAsync();
                connection.On<string>("HeartBeat", (message) =>
                {
                    Debug.WriteLine($"{message}");
                });
                connection.On<string>("ReceiveMessage", (msg) =>
                {
                    Debug.WriteLine($"{msg}");
                    OnMessageReceived.Invoke(this, new MessageArgs()
                    {
                        Messsage = msg.ToObj<PushMessage>()
                    });
                });
                HeartBeatAsync(connection);

                Debug.WriteLine($"started");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private Task Connection_Closed(Exception arg)
        {
            throw new NotImplementedException();
        }

        public void DisconnectAsync()
        {
            connection = null;
        }
        public async Task<bool> SendMessageAsync(string msg)
        {
            if (connection != null)
            {
                await connection.InvokeAsync("Message", msg);

                return true;
            }
            return false;
        }
        public void HeartBeatAsync(HubConnection connection)
        {
            TimeSpan period = TimeSpan.FromSeconds(60);

            ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
            {
                //
                // TODO: Work
                //
                await connection.InvokeAsync(HeartBeatMethodName, HeartBeat);
                //
                // Update the UI thread by using the UI core dispatcher.
                //
                //    Dispatcher.RunAsync(CoreDispatcherPriority.High,
                //        () =>
                //        {
                //    //
                //    // UI components can be accessed within this scope.
                //    //

                //});

            }, period);

        }
    }
}
