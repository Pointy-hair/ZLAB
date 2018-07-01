using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zlab.DataCore;
using Zlab.Test.Models;

namespace Zlab.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var usertoken = Login().GetAwaiter().GetResult();
            ConnectHubs(usertoken.sockettoken, "5b3894d039e68b614c8379f6", usertoken.token).Wait();
            Console.ReadLine();
        }
        static async Task ConnectHubs(string url, string userid, string token)
        {
            //var token = "10d33bc4a2d54b4997facc76e9485ede";

            var connection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();
            connection.Closed += Connection_Closed;


            try
            {
                await connection.StartAsync();
                connection.On<string>("HeartBeat", (message) =>
                {
                    Console.WriteLine($"{message}");
                });
                connection.On<string>("ReceiveMessage", (msg) =>
                 {
                     Console.WriteLine($"{msg}");
                 });
                await connection.InvokeAsync("HeartBeat", "::");
                System.Timers.Timer t = new System.Timers.Timer(10000);//实例化Timer类，设置间隔时间为10000毫秒；
                t.Elapsed += new System.Timers.ElapsedEventHandler(async (object source, System.Timers.ElapsedEventArgs e) =>
                {
                    await connection.InvokeAsync("HeartBeat", "::");
                });//到达时间的时候执行事件；
                t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
                t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；

                System.Timers.Timer t2 = new System.Timers.Timer(3000);
                t2.Elapsed += new System.Timers.ElapsedEventHandler(async (object source, System.Timers.ElapsedEventArgs e) =>
                 {
                     
                        await SendMessage(userid, "hello there", token);
                 });
                t2.AutoReset = false;//设置是执行一次（false）还是一直执行(true)；
                t2.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
                Console.WriteLine($"started");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static Task Connection_Closed(Exception arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }

        static async Task SendEmail()
        {

        }
        static async Task SignUp()
        {
             
        }
        static async Task ConnectWsAsync(string userid, string token)
        {
            var url = $"ws://zlab.yixinin.xyz/ws/websocket";//?token={token}&userid={userid}";
            //using (var ws = new WebSocket(url))
            //{ 
            //    ws.OnMessage += (sender, e) =>
            //        Console.WriteLine("Laputa says: " + e.Data);

            //    ws.Connect();
            //    ws.Send("::");
            //    Console.ReadKey(true);
            // }
            try
            {
                var socket = new ClientWebSocket();
                await socket.ConnectAsync(new Uri(url), CancellationToken.None);
                var bsend = Encoding.UTF8.GetBytes("::");
                await socket.SendAsync(new ArraySegment<byte>(bsend), WebSocketMessageType.Text, true, default(CancellationToken));
                while (true)
                {
                    ArraySegment<byte> buffer = default(ArraySegment<byte>);
                    await socket.ReceiveAsync(buffer, default(CancellationToken));
                    var msg = Encoding.UTF8.GetString(buffer);
                    Console.WriteLine(msg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
        static async Task<UserToken> Login()
        {
            var signup_url = "http://zlab.yixinin.xyz/api/account/signin";
            var socket_url = "http://zlab.yixinin.xyz/api/msg/socketurl";
            var body = new SigninModel()
            {
                password = "www999799",
                username = "990824559@qq.com",
            };

            var sign = await HttpHelper.PostAsync(signup_url, body.ToJson());

            var usertoken = sign.ToObj<ReturnResult<UserToken>>();

            var socket_token = await HttpHelper.GetAsync(
                $"{socket_url}?userid={usertoken.data.userid}&token={usertoken.data.token}");
            var sockettoken = socket_token.ToObj<ReturnResult<string>>();
            return new UserToken
            {
                userid = usertoken.data.userid,
                sockettoken = sockettoken.data,
                token = usertoken.data.token
            };
        }

        static async Task SendMessage(string userid, string body, string token)
        {
            var url = "http://zlab.yixinin.xyz/api/msg/sendmsg";
            var msg = new SendMsgModel()
            {
                token = token,
                ats = new List<string>(),
                group = MessageGroup.Personal,
                imgs = new List<string>(),
                message = body,
                touserids = new List<string>() { userid },
                type = MessageType.text
            };
            var res = await HttpHelper.PostAsync(url, msg.ToJson());
            Console.WriteLine(res);
        }
    }
}
