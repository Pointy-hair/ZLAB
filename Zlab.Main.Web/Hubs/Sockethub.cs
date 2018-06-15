using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Zlab.DataCore.DbCore;
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
            }
          
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} joined");
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

            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} left");
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

        private async Task<bool> SetSocketClientAsync(string id, string userid)
        {
            var redis = RedisCore.GetClient();
            return await redis.StringSetAsync($"{Keys.socket_clients_prefix}{id}", userid);
        }


        private async Task<string> GetSocketClientUserIdAsync(string id)
        {
            var redis = RedisCore.GetClient();
            var userid = await redis.StringGetAsync(Keys.socket_clients_prefix);

            return userid;
        }
    }
}
