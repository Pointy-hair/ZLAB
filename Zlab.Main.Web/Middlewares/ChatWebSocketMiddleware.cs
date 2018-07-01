using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zlab.DataCore;
using Zlab.DataCore.DbCore;
using Zlab.DataCore.Entities;
using Zlab.Main.Web.Hubs;
using Zlab.Main.Web.Models;
using Zlab.Main.Web.Services.Implements;
using Zlab.Main.Web.Services.Interfaces;

namespace Zlab.Main.Web.MidWares
{
    public class ChatWebSocketMiddleware
    {
        private readonly static string HeartBeat = "::";
        public static ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        private readonly RequestDelegate _next;
        private readonly ISessionManager sessionManager;

        public ChatWebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
            sessionManager = new SessionManager();
        }


        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }

            CancellationToken ct = context.RequestAborted;
            var currentSocket = await context.WebSockets.AcceptWebSocketAsync();
            //string socketId = Guid.NewGuid().ToString();
            string userid = context.Request.Query["userid"].ToString();
            string token = context.Request.Query["token"].ToString();
            if (!_sockets.ContainsKey(userid))
            {
                //var checkuserid = await sessionManager.GetSocketUserIdAsync(token);
                //if (checkuserid != userid)
                //{
                //    await currentSocket.CloseAsync(WebSocketCloseStatus.Empty, "access denined", CancellationToken.None);
                //    return;
                //}
                //await PushMessageAsync(userid, currentSocket);

                _sockets.TryAdd(userid, currentSocket);
            }
            //_sockets.TryRemove(socketId, out dummy);
            //_sockets.TryAdd(socketId, currentSocket);

            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }
                string response = await ReceiveStringAsync(currentSocket, ct);


                if (string.IsNullOrEmpty(response))
                {
                    if (currentSocket.State != WebSocketState.Open)
                    {
                        break;
                    }

                    continue;
                }
                //心跳包 直接返回
                if (response == HeartBeat)
                {
                    await SendStringAsync(currentSocket, HeartBeat, ct);
                    continue;
                }
                else if(response.Contains("uid"))
                {
                    var rtc = response.ToObj<RTCMessage>();

                    foreach (var socket in _sockets)
                    {
                        if (socket.Value.State != WebSocketState.Open)
                        {
                            continue;
                        }
                        if (socket.Key == rtc.uid)
                        {
                            var body = new PushMessage()
                            {
                                RtcBody = rtc.msg,
                                type = PushType.Rtc
                            };

                            await SendStringAsync(socket.Value, body.ToJson(), ct);
                        }
                    }
                }
                else
                {
                    await SendStringAsync(currentSocket, response, ct);
                }

            }

            //_sockets.TryRemove(socketId, out dummy);

            await currentSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", ct);
            currentSocket.Dispose();
        }

        private static async Task PushMessageAsync(string userid, WebSocket socket)
        {
            var filter = Builders<UserMessage>.Filter;
            var filters = filter.Eq(x => x.UserId, userid)
                & filter.Eq(x => x.Read, false);
            var repo = new MongoCore<UserMessage>();
            var msgids = await repo.Collection.Find(filters).Project(x => x.MessageId).ToListAsync();
            
            if (msgids != null && msgids.Any())
            {
                var msg = new PushMessage()
                {
                    type = PushType.MessageId,
                    msgids = msgids
                };
                await SendStringAsync(socket, msg.ToJson());
            }

        }

        public static Task SendStringAsync(WebSocket socket, string data, CancellationToken ct = default(CancellationToken))
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            var segment = new ArraySegment<byte>(buffer);
            return socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
        }

        private static async Task<string> ReceiveStringAsync(WebSocket socket, CancellationToken ct = default(CancellationToken))
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    ct.ThrowIfCancellationRequested();

                    result = await socket.ReceiveAsync(buffer, ct);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);
                if (result.MessageType != WebSocketMessageType.Text)
                {
                    return null;
                }

                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}
