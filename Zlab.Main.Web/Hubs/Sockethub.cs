using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using Zlab.DataCore;
using Zlab.DataCore.DbCore;
using Zlab.DataCore.Entities;
using Zlab.Main.Web.Models;
using Zlab.Main.Web.Services.Implements;
using Zlab.Main.Web.Services.Interfaces;

namespace Zlab.Main.Web.Hubs
{
    public class SocketHub : Hub
    {

        private static SocketHub socket;
        private readonly ISessionManager sessionManager;
        public Dictionary<String, List<string>> Users { get; set; }

        public SocketHub() : base()
        {
            socket = this;
            sessionManager = new SessionManager();
        }

        public static SocketHub GetInstans()
        {
            return socket;
        }


        /// <summary>
        /// 建立连接时触发
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            var token = Context.Items["token"] as string;
            if (!string.IsNullOrEmpty(token))
            {
                var userid = await sessionManager.GetUserIdAsync(token);
                var socketid = this.Context.ConnectionId;
                if (Users.ContainsKey(userid))
                    Users[userid].Add(socketid);
                else
                    Users.Add(userid, new List<string>() { socketid });
                await PushMessageAsync(userid);
            }
        }

        /// <summary>
        /// 离开连接时触发
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception ex)
        {
            var socketid = Context.ConnectionId;
            string userid = Users.First(x => x.Value.Contains(socketid)).Key;

            if (string.IsNullOrEmpty(userid))
            {
                if (Users.ContainsKey(userid))
                    Users[userid].Remove(socketid);
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// 向所有人推送消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task Send(string message)
        {
            return Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId}: {message}");
        }
        /// <summary>
        /// 向指定组推送消息
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendToGroup(string groupName, string message)
        {
            return Clients.Group(groupName).SendAsync("ReceiveMessage", $"{Context.ConnectionId}@{groupName}: {message}");
        }
        /// <summary>
        /// 加入指定组并向组推送消息
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task JoinGroup(string groupName)
        {

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} joined {groupName}");
        }
        /// <summary>
        /// 推出指定组并向组推送消息
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} left {groupName}");
        }
        /// <summary>
        /// 向指定Id推送消息
        /// </summary>
        /// <param name="userid">要推送消息的对象</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task Echo(string userid, string message)
        {
            return Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", $"{Context.ConnectionId}: {message}");
        }
        /// <summary>
        /// 向所有人推送消息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string user, string message)
        {

            await Clients.All.SendAsync("ReceiveMessage", Context.ConnectionId, message);

        }

        private async Task<List<string>> GetSocketClientIdAsync(string userid)
        {
            var ids = Users.First(x => x.Key == userid).Value;
            return await Task.FromResult(ids);
        }
        private async Task PushMessageAsync(string userid)
        {
            var filter = Builders<Message>.Filter;
            var filters = filter.AnyEq(x => x.ToUserIds, userid)
                & filter.Eq(x => x.IsRead, false)
                & filter.Eq(x => x.IsRealTime, false);
            var repo = new MongoCore<Message>();
            var msgids = await repo.Collection.Find(filters).Project(x => x.Id).ToListAsync();
            var msg = new PushMessage()
            {
                type = PushType.MessageId,
                msgids = msgids
            };
            var clientids = await GetSocketClientIdAsync(userid);
            var clients = Clients.Clients(clientids);
            await clients.SendAsync("ReceiveMessage", msg.ToJson());
        }
        public async Task PushMessageAsync(string userid, string body, PushType type)
        {
            var msg = new PushMessage()
            {
                type = type,
                RtcBody = type == PushType.Rtc ? body : string.Empty,
                msgids = type == PushType.MessageId ? new List<string>() { body } : new List<string>(),
            };
            var clientids = await GetSocketClientIdAsync(userid);
            var clients = Clients.Clients(clientids);
            await clients.SendAsync("ReceiveMessage", msg.ToJson());
        }
    }
}
