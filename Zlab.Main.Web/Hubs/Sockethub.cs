using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
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
        private readonly ISessionManager sessionManager;
        public static ConcurrentDictionary<string, List<string>> Users = new ConcurrentDictionary<string, List<string>>();
        public static ConcurrentDictionary<string, List<string>> Channels = new ConcurrentDictionary<string, List<string>>();
        public SocketHub() : base()
        {
            sessionManager = new SessionManager();
        }
        /// <summary>
        /// 建立连接时触发
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            var socketid = this.Context.ConnectionId;
            //var clinet = Clients.Client(Context.ConnectionId);

            string token = Context.GetHttpContext().Request.Query["token"];
            if (!string.IsNullOrEmpty(token))
            {
                var userid = await sessionManager.GetSocketUserIdAsync(token);
                if (!string.IsNullOrEmpty(userid))
                {
                    if (Users.ContainsKey(userid))
                        Users[userid].Add(socketid);
                    else
                        Users.TryAdd(userid, new List<string>() { socketid });

                    var channelids = await GetUserChannelIdsAsync(userid);
                    if (channelids != null && channelids.Any())
                    {
                        foreach (var channelid in channelids)
                        {
                            await Groups.AddToGroupAsync(Context.ConnectionId, channelid);
                        }
                    }

                    //await PushMessageAsync(userid, Clients.Caller);
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                throw new Exception();
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

            if (!string.IsNullOrEmpty(userid))
            {
                if (Users.ContainsKey(userid))
                    Users[userid].Remove(socketid);
                var channelids = await GetUserChannelIdsAsync(userid);
                if (channelids != null && channelids.Any())
                {
                    foreach (var channelid in channelids)
                    {
                        await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelid);
                    }
                }
            }
            await Task.CompletedTask;
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

            //await Clients.Group(groupName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} joined {groupName}");
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


        private async Task<List<string>> GetSocketClientIdAsync(string userid)
        {
            var ids = Users.First(x => x.Key == userid).Value;

            return await Task.FromResult(ids);
        }

        private async Task<IList<string>> GetUserChannelIdsAsync(string userid)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Id, userid);
            var repo = new MongoCore<User>();
            var channelids = await repo.Collection.Find(filter).Project(x => x.ChannelIds).FirstOrDefaultAsync();
            return channelids;
        }

        private async Task PushMessageAsync(string userid, IClientProxy client)
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
                try
                {
                    client.SendAsync("ReceiveMessage", msg.ToJson());
                }
                catch (Exception)
                {

                }
            }
        }
        public async Task<List<string>> PushMessageAsync(string userid, string body, PushType type)
        {
            var msg = new PushMessage()
            {
                type = type,
                RtcBody = type == PushType.Rtc ? body : string.Empty,
                msgids = type == PushType.MessageId ? new List<string>() { body } : new List<string>(),
            };
            var clientids = await GetSocketClientIdAsync(userid);
            if (clientids != null && clientids.Any())
            {
                var removeids = new List<string>();
                foreach (var clientid in clientids)
                {
                    try
                    {
                        var client = Clients.Client(clientid);
                        client.SendAsync("ReceiveMessage", msg.ToJson());
                    }
                    catch (Exception)
                    {
                        removeids.Add(clientid);
                    }
                }

                foreach (var clientid in removeids)
                {
                    //if (Users.ContainsKey(userid))
                    //    Users[userid].Remove(clientid);
                    //clientids.Remove(clientid);
                }
            }
            return clientids ?? new List<string>();
        }

        public Task HeartBeat(string message)
        {
            //Clients.Caller.SendAsync("ReceiveMessage", Users.ToJson());
            //Clients.Clients(new List<string> { Context.ConnectionId }).SendAsync("ReceiveMessage", Context.ConnectionId);

            return Clients.Caller.SendAsync("HeartBeat", message);
        }
        public async Task Test(IClientProxy client, string userid)
        {

            await client.SendAsync("ReceiveMessage", userid);
        }

    }
}
