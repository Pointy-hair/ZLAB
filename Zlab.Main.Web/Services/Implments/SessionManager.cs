using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zlab.DataCore.DbCore;
using Zlab.Main.Web.Services.Interfaces;
using Zlab.UtilsCore;
using Zlab.Web.Main.Services.Interfaces;

namespace Zlab.Main.Web.Services.Implements
{
    public class SessionManager : ISessionManager
    {
        private readonly string key_pre = "sess:";
        private readonly string device_set = "device:";
        public async Task<string> GetSessionAsync(string userid)
        {
            var redis = RedisCore.GetClient();
            return await redis.StringGetAsync($"{key_pre}{userid}");
        }

        public async Task<string> ReCacheSessionAsync(string userid)
        {
            var session = GuidHelper.GetGuid();
            var redis = RedisCore.GetClient();
            if (await redis.StringSetAsync($"{key_pre}{userid}", session))
                return session;
            return string.Empty;
        }
        public async Task<bool> AddDeviceAsync(string userid, string device)
        {
            var redis = RedisCore.GetClient();
            return await redis.SetRemoveAsync($"{device_set}{userid}", device);
        }
        public async Task<bool> RemoveDeviceAsync(string userid, string device)
        {
            var redis = RedisCore.GetClient();
            return await redis.SetAddAsync($"{device_set}{userid}", device);
        }
    }
}
