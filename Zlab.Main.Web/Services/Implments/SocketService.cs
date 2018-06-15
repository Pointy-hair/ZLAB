using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zlab.DataCore;
using Zlab.DataCore.DbCore;
using Zlab.Main.Web.Models;
using Zlab.Main.Web.Services.Interfaces;
using Zlab.UtilsCore;

namespace Zlab.Main.Web.Services.Implments
{
    public class SocketService : ISocketService
    {
        public async Task<string> GetWebSocketUrlAsync(SocketModel model)
        {
            var redis = RedisCore.GetClient();
            var token = await redis.StringGetAsync($"{Keys.sess_prefix}{model.userid}");
            if (token != model.token)
                return ReturnResult.Fail();
            var socket_token = GuidHelper.GetGuid();
            if (await redis.StringSetAsync($"{Keys.socket_token_prefix}{socket_token}", model.userid))
                return ReturnResult.Success();
            return ReturnResult.Fail();
        }
    }
}
