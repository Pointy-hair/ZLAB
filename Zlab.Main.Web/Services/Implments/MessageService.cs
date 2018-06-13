using System.Collections.Generic;
using System.Threading.Tasks;
using Zlab.DataCore.DbCore;
using Zlab.DataCore.Entities;
using Zlab.Main.Web.Services.Interfaces;

namespace Zlab.Main.Web.Services.Implements
{
    public class MessageService : IMessageService
    {
        public async Task<bool> SendMessageAsync(List<string> touserids, string message)
        {
            var repo = new MongoCore<Message>();
            await repo.Collection.InsertOneAsync(new Message()
            {
                Body = message,
                ToUserIds = touserids
            });
            return true;
        }
    }
}
