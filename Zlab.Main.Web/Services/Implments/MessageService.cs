using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zlab.DataCore.DbCore;
using Zlab.DataCore.Entities;
using Zlab.Main.Web.Hubs;
using Zlab.Main.Web.Models;
using Zlab.Main.Web.Services.Interfaces;
using Zlab.UtilsCore;

namespace Zlab.Main.Web.Services.Implements
{
    public class MessageService : IMessageService
    {
        private readonly ISessionManager sessionManager;
        public MessageService(ISessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }
        public async Task<bool> SendMessageAsync(SendMsgModel model)
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
            var repo = new MongoCore<Message>();
            await repo.Collection.InsertOneAsync(msg);
            await SocketHub.GetInstans().PushMessageAsync(msg.Sender, msg.Id, PushType.MessageId);
            return true;
        }
    }
}
