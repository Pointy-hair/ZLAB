using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zlab.DataCore.DbCore;
using Zlab.Main.Web.Models;
using Zlab.Main.Web.Services.Interfaces;
using Zlab.UtilsCore;
using Zlab.Web.Main.Services.Interfaces;

namespace Zlab.Main.Web.Services.Implements
{
    public class SessionManager : ISessionManager
    {
        private readonly string key_pre = "sess:";
        private readonly string device_set = "device:";
        private readonly string invite_pre = "invite:";
        public async Task<string> GetUserIdAsync(string token)
        {
            var redis = RedisCore.GetClient();
            return await redis.StringGetAsync($"{key_pre}{token}");
        }

        public async Task<string> ReCacheSessionAsync(string userid)
        {
            var token = GuidHelper.GetGuid();
            var redis = RedisCore.GetClient();
            if (await redis.StringSetAsync($"{key_pre}{token}", userid, TimeSpan.FromHours(1)))
                return token;
            return string.Empty;
        }

        public async Task<string> GetSocketUserIdAsync(string token)
        {
            var redis = RedisCore.GetClient();
            return await redis.StringGetAsync($"{Keys.socket_token_prefix}{token}");
        }

        public async Task<string> ReCacheSocketSessionAsync(string userid)
        {
            var token = GuidHelper.GetGuid();
            var redis = RedisCore.GetClient();
            if (await redis.StringSetAsync($"{Keys.socket_token_prefix}{token}", userid, TimeSpan.FromHours(1)))
                return token;
            return string.Empty;
        }

        public async Task<bool> AddDeviceAsync(string userid, string deviceModel, string deviceName)
        {
            var redis = RedisCore.GetClient();
            return await redis.SetRemoveAsync($"{device_set}{userid}", $"{deviceModel},{deviceName}");
        }
        public async Task<bool> RemoveDeviceAsync(string userid, string deviceModel, string deviceName)
        {
            var redis = RedisCore.GetClient();
            return await redis.SetAddAsync($"{device_set}{userid}", $"{deviceModel},{deviceName}");
        }

        public async Task<bool> CheckUserAsync(string userid, string token)
        {
            var sess = await GetSocketUserIdAsync(token);
            return sess == userid;
        }

        public async Task<string> CacheInviteTokenAsync(string channelid)
        {
            var token = GuidHelper.GetGuid();
            var key = $"{invite_pre}{token}";
            var redis = RedisCore.GetClient();
            var seted = await redis.StringSetAsync(key, channelid);
            return seted ? token : string.Empty ;
        }

        public async Task<string> GetChannelIdByTokenAsync(string invitetoken)
        {
            var token = GuidHelper.GetGuid();
            var key = $"{invite_pre}{invitetoken}";
            var redis = RedisCore.GetClient();
            var channelid = await redis.StringGetAsync(key);
            return channelid;
        }
    }
}
