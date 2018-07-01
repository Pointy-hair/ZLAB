using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zlab.DataCore;
using Zlab.DataCore.DbCore;
using Zlab.DataCore.Entities;
using Zlab.Main.Web.Hubs;
using Zlab.Main.Web.MidWares;
using Zlab.Main.Web.Models;
using Zlab.Main.Web.Services.Interfaces;
using Zlab.UtilsCore;

namespace Zlab.Main.Web.Services.Implements
{
    public class MessageService : IMessageService
    {
        private readonly ISessionManager sessionManager;
        private static readonly string SocketUrl = "http://zlab.yixinin.xyz/websocket?token=";
        // public MessageService() { }
        public MessageService(ISessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }
        public async Task<string> SendMessageAsync(SendMsgModel model, IHubContext<SocketHub> hubContext)
        {
            var imgpaths = new List<string>();
            if (model.imgs != null && model.imgs.Any())
            {

            }
            var msg = new Message()
            {
                Body = model.message,
                ToUserIds = model.touserids,
                Type = model.type,
                AtUserIds = model.ats,
                CreateTime = TimeHelper.GetUnixTimeMilliseconds(),
                Group = model.group,
                Sender = await sessionManager.GetUserIdAsync(model.token),
                ImagePaths = imgpaths
            };
            var usermsgs = new List<UserMessage>();
            foreach (var userid in model.touserids)
            {
                usermsgs.Add(new UserMessage()
                {
                    MessageId = msg.Id,
                    Read = false,
                    UserId = userid
                });
            }

            var repo = new MongoCore<Message>();
            var usermsgrepo = new MongoCore<UserMessage>();
            await repo.Collection.InsertOneAsync(msg);
            await usermsgrepo.Collection.InsertManyAsync(usermsgs);
            var body = new PushMessage()
            {
                msgids = new List<string>() { msg.Id },
                type = PushType.MessageId
            };
            var sockets = ChatWebSocketMiddleware._sockets.Where(x => model.touserids.Contains(x.Key)).ToList();
            if (sockets != null && sockets.Any())
            {
                foreach (var sock in sockets)
                {
                    await ChatWebSocketMiddleware.SendStringAsync(sock.Value, body.ToJson());
                }
            }
            var sents = new List<string>();
            var users = SocketHub.Users.Where(x => model.touserids.Contains(x.Key)).ToList();
            if (users != null && users.Any())
            {
                foreach (var user in users)
                {
                    var clientids = user.Value ?? new List<string>();
                    if (clientids.Any())
                    {
                        foreach (var clientid in clientids)
                        {
                            try
                            {
                                var client = hubContext.Clients.Client(clientid);
                                if (client != null)
                                {
                                    await client.SendAsync("ReceiveMessage", body.ToJson());
                                    sents.Add(clientid);
                                }

                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }
                }
            }
            return ReturnResult.Success(sents);
        }
        public async Task<string> GetMessagesAsync(string[] msgid)
        {
            var filter = Builders<Message>.Filter.In(x => x.Id, msgid);
            var repo = new MongoCore<Message>();
            var msg = await repo.Collection.Find(filter).Project(x => new UserMessageDto
            {
                AtUserIds = x.AtUserIds,
                Body = x.Body,
                CreateTime = x.CreateTime,
                Group = x.Group,
                Id = x.Id,
                ImagePaths = x.ImagePaths,
                Sender = x.Sender,
                ToUserIds = x.ToUserIds,
                Type = x.Type
            }).ToListAsync();
            return ReturnResult.Success(msg);
        }
        public async Task<string> SetMessagesReadAsync(string[] msgid)
        {
            var filter = Builders<UserMessage>.Filter.In(x => x.MessageId, msgid);
            var update = Builders<UserMessage>.Update.Set(x => x.Read, true);
            var repo = new MongoCore<UserMessage>();
            await repo.Collection.UpdateManyAsync(filter, update);
            return ReturnResult.Success();
        }

        public async Task<string> GetWebSocketUrlAsync(UserTokenDto model)
        {
            var redis = RedisCore.GetClient();
            var userid = await sessionManager.GetUserIdAsync(model.token);
            if (userid != model.userid)
                return ReturnResult.Fail("token error");
            var socket_token = await sessionManager.ReCacheSocketSessionAsync(userid);
            if (!string.IsNullOrEmpty(socket_token))
                return ReturnResult.Success($"{SocketUrl}{socket_token}");
            return ReturnResult.Fail("set socketkey error");
        }

        public async Task<string> RollBackMessageASync(string msgid, IHubContext<SocketHub> hubContext)
        {
            var usermsgrepo = new MongoCore<UserMessage>();
            var usermsgfilter = Builders<UserMessage>.Filter.Eq(x => x.MessageId, msgid);
            await usermsgrepo.Collection.DeleteManyAsync(usermsgfilter);


            var filter = Builders<Message>.Filter;
            var filters = filter.Eq(x => x.Id, msgid)
                & filter.Lt(x => x.CreateTime, DateTimeOffset.UtcNow.AddMinutes(-3).ToUnixTimeMilliseconds());
            var repo = new MongoCore<Message>();
           
            var userids = await repo.Collection.Find(filters).Project(x => x.ToUserIds).FirstOrDefaultAsync();

           
            var body = new PushMessage
            {
                msgids = new List<string>() { msgid },
                type = PushType.MessageBack
            };
            if (userids != null && userids.Any())
            {
                var users = SocketHub.Users.Where(x => userids.Contains(x.Key)).ToList();
                if (users != null && users.Any())
                {
                    foreach (var user in users)
                    {
                        var clientids = user.Value ?? new List<string>();
                        if (clientids.Any())
                        {
                            foreach (var clientid in clientids)
                            {
                                try
                                {
                                    var client = hubContext.Clients.Client(clientid);
                                    if (client != null)
                                    {
                                        await client.SendAsync("ReceiveMessage", body.ToJson());
                                    }

                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
                return ReturnResult.Success();
            }
            else
                return ReturnResult.Fail("time out");
        }
    }
}
