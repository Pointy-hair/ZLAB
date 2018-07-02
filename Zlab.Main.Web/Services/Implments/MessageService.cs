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
        private static readonly string PushMessageName = "ReceiveMessage";
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
                ToUserId = model.touserid ?? string.Empty,
                Type = model.type,
                AtUserIds = model.ats,
                CreateTime = TimeHelper.GetUnixTimeMilliseconds(),
                Group = model.group,
                UserId = await sessionManager.GetUserIdAsync(model.token),
                ImagePaths = imgpaths,
                ToChannelId = model.tochannelid ?? string.Empty
            };
            var repo = new MongoCore<Message>();
            await repo.Collection.InsertOneAsync(msg);


            var usermsgs = new List<UserMessage>();
            ChannelMessage channelmsg = null;
            //channel
            if (!string.IsNullOrEmpty(model.tochannelid))
            {
                // add to user message
                var cfilter = Builders<Channel>.Filter.Eq(x => x.Id, model.tochannelid);
                var channelrepo = new MongoCore<Channel>();
                var userids = await channelrepo.Collection.Find(cfilter).Project(x => x.UserIds).FirstOrDefaultAsync();
                if (userids != null && userids.Any())
                {
                    foreach (var userid in userids)
                    {
                        usermsgs.Add(new UserMessage()
                        {
                            MessageId = msg.Id,
                            Read = false,
                            UserId = userid
                        });
                    }
                }

                // add to channel message
                channelmsg = new ChannelMessage
                {
                    ChannelId = msg.ToChannelId,
                    FromUserId = msg.UserId,
                    MessageId = msg.Id
                };
            }
            // user
            else if (!string.IsNullOrEmpty(model.touserid))
            {
                usermsgs.Add(new UserMessage()
                {
                    MessageId = msg.Id,
                    Read = false,
                    UserId = msg.ToUserId
                });
            }
            // add user msg 
            if (usermsgs != null && usermsgs.Any())
            {
                var usermsgrepo = new MongoCore<UserMessage>();
                await usermsgrepo.Collection.InsertManyAsync(usermsgs);
            }

            //add channel msg 
            if (channelmsg != null)
            {
                var crepo = new MongoCore<ChannelMessage>();
                await crepo.Collection.InsertOneAsync(channelmsg);
            }

            var body = new PushMessage()
            {
                msgids = new List<string>() { msg.Id },
                type = PushType.MessageId
            };
            //var sockets = ChatWebSocketMiddleware._sockets.Where(x => model.touserids.Contains(x.Key)).ToList();
            //if (sockets != null && sockets.Any())
            //{
            //    foreach (var sock in sockets)
            //    {
            //        await ChatWebSocketMiddleware.SendStringAsync(sock.Value, body.ToJson());
            //    }
            //}
            if (channelmsg != null && !string.IsNullOrEmpty(msg.ToChannelId))
            {
                await hubContext.Clients.Group(msg.ToChannelId).SendAsync(PushMessageName, body.ToJson());
            }
            var sents = new List<string>();
            var users = SocketHub.Users.Where(x => usermsgs.Select(y => y.UserId).Contains(x.Key));
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
                                    await client.SendAsync(PushMessageName, body.ToJson());
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
        public async Task<string> GetMessagesAsync(string[] msgids)
        {
            var filter = Builders<Message>.Filter.In(x => x.Id, msgids);
            var repo = new MongoCore<Message>();
            var msg = await repo.Collection.Find(filter).Project(x => new UserMessageDto
            {
                atuserids = x.AtUserIds,
                body = x.Body,
                createtime = x.CreateTime,
                group = x.Group,
                id = x.Id,
                imagepaths = x.ImagePaths,
                sender = x.UserId,
                type = x.Type
            }).ToListAsync();
            var usermsgrepo = new MongoCore<UserMessage>();
            await usermsgrepo.Collection.UpdateManyAsync(Builders<UserMessage>.Filter.In(x => x.MessageId, msgids), Builders<UserMessage>.Update.Set(x => x.Read, true));
            return ReturnResult.Success(msg);
        }
        public async Task<string> GetMessagesAsync(string userid)
        {
            var filter = Builders<UserMessage>.Filter.Eq(x => x.Read, false) & Builders<UserMessage>.Filter.Eq(x => x.UserId, userid);
            var repo = new MongoCore<UserMessage>();
            var msgids = await repo.Collection.Find(filter).Project(x => x.MessageId).ToListAsync();
            if (msgids != null && msgids.Any())
            {
                return await GetMessagesAsync(msgids.ToArray());
            }
            return ReturnResult.Fail("no msgs");
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
            var filter = Builders<Message>.Filter;
            var filters = filter.Eq(x => x.Id, msgid)
                & filter.Lt(x => x.CreateTime, DateTimeOffset.UtcNow.AddMinutes(-3).ToUnixTimeMilliseconds());
            var repo = new MongoCore<Message>();

            var to = await repo.Collection.Find(filters).Project(x => new { x.ToUserId, x.ToChannelId }).FirstOrDefaultAsync();


            var body = new PushMessage
            {
                msgids = new List<string>() { msgid },
                type = PushType.MessageBack
            };
            if (string.IsNullOrEmpty(to?.ToUserId))
            {
                var usermsgrepo = new MongoCore<UserMessage>();
                var usermsgfilter = Builders<UserMessage>.Filter.Eq(x => x.MessageId, msgid);
                await usermsgrepo.Collection.DeleteManyAsync(usermsgfilter);

                var users = SocketHub.Users.Where(x => to.ToUserId == x.Key).ToList();
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
            }
            else if (string.IsNullOrEmpty(to?.ToChannelId))
            {
                var channelmsgrepo = new MongoCore<ChannelMessage>();
                var channelmsgfilter = Builders<ChannelMessage>.Filter.Eq(x => x.MessageId, msgid);
                await channelmsgrepo.Collection.DeleteOneAsync(channelmsgfilter);
                await hubContext.Clients.Group(to.ToChannelId).SendAsync("ReceiveMessage", body.ToJson());
            }
            else
                return ReturnResult.Fail("time out");
            return ReturnResult.Success();
        }

        public async Task<string> AddToChannelAsync(AddChannelModel model)
        {
            if (!string.IsNullOrEmpty(model.invatetoken))
            {
                var channelid = await sessionManager.GetChannelIdByTokenAsync(model.invatetoken);
                if (channelid != model.channelid)
                    return ReturnResult.Fail("invaid token");
            }
            var filter = Builders<Channel>.Filter;
            var filters = filter.Eq(x => x.Id, model.channelid)
                & filter.Eq(x => x.IsPrivate, !string.IsNullOrEmpty(model.invatetoken));
            var update = Builders<Channel>.Update.Push(x => x.UserIds, model.userid);
            var repo = new MongoCore<Channel>();
            var updated = await repo.Collection.UpdateOneAsync(filters, update);
            if (updated.ModifiedCount == 1)
            {
                var userfilter = Builders<User>.Filter.Eq(x => x.Id, model.userid);
                var userupdate = Builders<User>.Update.Push(x => x.ChannelIds, model.channelid);
                var urepo = new MongoCore<User>();
                await urepo.Collection.UpdateOneAsync(userfilter, userupdate);
            }
            return ReturnResult.Fail();
        }

        public async Task<string> RemoveChannelAsync(RemoveChannelModel model)
        {
            if (model.removeuserids != null && model.removeuserids.Any())
            {
                var filter = Builders<Channel>.Filter;
                var filters = filter.Eq(x => x.Id, model.channelid);
                var update = Builders<Channel>.Update.PullAll(x => x.UserIds, model.removeuserids);
                var repo = new MongoCore<Channel>();
                if (model.removeuserids.Length > 1 || model.removeuserids.Contains(model.userid))
                {
                    filters = filters & filter.Eq(x => x.Owner, model.userid);
                    if (model.removeuserids.Contains(model.userid) && string.IsNullOrEmpty(model.moveuserid))
                    {
                        filters = filters & filter.AnyEq(x => x.UserIds, model.moveuserid);
                        update = update.Set(x => x.Owner, model.moveuserid);
                    } 
                }
                else
                {
                    filters = filters & filter.AnyEq(x => x.UserIds, model.removeuserids.First()); 
                }
                var updated = await repo.Collection.UpdateOneAsync(filters, update);
                if (updated.ModifiedCount <= 0)
                {
                    return ReturnResult.Fail();
                }

                var ufilter = Builders<User>.Filter.In(x => x.Id, model.removeuserids);
                var uupdate = Builders<User>.Update.Pull(x => x.ChannelIds, model.channelid);
                var urepo = new MongoCore<User>();
                await urepo.Collection.UpdateManyAsync(ufilter, uupdate);

                return ReturnResult.Success();
            }
            return ReturnResult.Fail("params error");
        }
        public async Task<string> InviteUserToChannelAsync(InviteChannelModel model, IHubContext<SocketHub> hubContext)
        {
            var filter = Builders<Channel>.Filter.Eq(x => x.Id, model.channelid);
            var repo = new MongoCore<Channel>();
            var channel = await repo.Collection.Find(filter).Project(x => new { x.Owner, x.IsPrivate }).FirstOrDefaultAsync();
            if (channel != null)
            {
                var channeltoken = string.Empty;
                if (channel.IsPrivate)
                {
                    if (model.userid != channel.Owner)
                    {
                        return ReturnResult.Fail("access denied");
                    }
                    channeltoken = await sessionManager.CacheInviteTokenAsync(model.channelid);
                    if (string.IsNullOrEmpty(channeltoken))
                    {
                        channeltoken = await sessionManager.CacheInviteTokenAsync(model.channelid);
                    }
                }
                var body = new PushMessage()
                {
                    type = PushType.ChannelInvite,
                    invite = $"{model.channelid},{channeltoken}"
                };
                var users = SocketHub.Users.Where(x => model.inviteuserids.Contains(x.Key));
                if (users != null && users.Any())
                {
                    foreach (var user in users)
                    {
                        var clientids = user.Value;
                        if (clientids != null && clientids.Any())
                        {
                            try
                            {
                                var clients = hubContext.Clients.Clients(clientids);
                                await clients.SendAsync(PushMessageName, body.ToJson());
                            }
                            catch (Exception ex)
                            {
                                LogHelper.Error(ex);
                            }
                        }
                    }
                }

                return ReturnResult.Success(channeltoken);
            }
            return ReturnResult.Fail();
        }
    }
}
