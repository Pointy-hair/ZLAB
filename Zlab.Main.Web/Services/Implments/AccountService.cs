using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zlab.DataCore;
using Zlab.DataCore.DbCore;
using Zlab.DataCore.Entities;
using Zlab.Main.Web.Models;
using Zlab.Main.Web.Service.Interfaces;
using Zlab.Main.Web.Services.Interfaces;
using Zlab.UtilsCore;
using Zlab.Web.Service.Implments;

namespace Zlab.Main.Web.Service.Implments
{
    public class AccountService : IAccountService
    {
        private readonly IIdsService idsService;
        private readonly ISessionManager sessionManager;
        // public AccountService() { }
        public AccountService(ISessionManager sessionManager, IIdsService idsService)
        {
            this.sessionManager = sessionManager;
            this.idsService = idsService;
        }

        public async Task<string> SignUpAsync(SignupModel model)
        {
            if (model.password.Length < 6)
                return ReturnResult.Fail("password too simple");
            var redis = RedisCore.GetClient();
            var code = await redis.StringGetAsync($"emailcode:{model.email}");
            if (string.IsNullOrEmpty(code))
                return ReturnResult.Fail("code not exsist");
            if (code != model.code)
                return ReturnResult.Fail("code error");
            if (idsService == null)
                return ReturnResult.Fail("idservice is null");
            var userid = await idsService.GetIdAsync("UserId");
            var user = new User
            {
                UserName = model.username,
                UserId = userid.ToString(),
                Emails = new List<string>() { model.email },
                CreateTime = TimeHelper.GetUnixTimeMilliseconds(),
                Passwords = model.password,
            };
            var repo = new MongoCore<User>();
            var count = await repo.Collection.CountAsync(Builders<User>.Filter.AnyEq(x => x.Emails, model.email));
            if (count > 0)
                return ReturnResult.Fail("email already registed");
            await repo.Collection.InsertOneAsync(user);
            return ReturnResult.Success(user.UserId);
        }

        public async Task<bool> SendEmailAsync(string email)
        {
            var code = GuidHelper.GetGuid().Substring(0, 4);
            var emailbody = $"your registry code: {code}, please use it in 10 mimutes. this is a system email, do not reply. thank you!";
            var sent = await EmailHelper.SendEmailAsync(email, emailbody, false);
            if (sent)
            {
                var redis = RedisCore.GetClient();
                var seted = await redis.StringSetAsync($"emailcode:{email}", code, TimeSpan.FromMinutes(10));
            }
            return sent;
        }

        public async Task<string> SigninAsync(SigninModel model)
        {
            var username = string.Empty;
            var userid = string.Empty;

            if (model.username.Contains('#'))
            {
                var account = model.username.Split('#');
                if (account.Length == 2)
                {
                    username = account[0] ?? string.Empty;
                    userid = account[1] ?? string.Empty;
                }
            }

            var filter = Builders<User>.Filter;
            var filters = filter.Eq(x => x.Passwords, model.password) &
                (filter.AnyEq(x => x.Emails, model.username)
                | filter.Eq(x => x.Phone, model.username)
                | (filter.Eq(x => x.UserName, username) & filter.Eq(x => x.UserId, userid)));
            var repo = new MongoCore<User>();
            var _id = await repo.Collection.Find(filters).Project(x => x.Id).FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(_id))
            {
                var token = await sessionManager.ReCacheSessionAsync(_id);
                if (!string.IsNullOrEmpty(token))
                    return ReturnResult.Success(new UserTokenDto
                    {
                        token = token,
                        userid = _id
                    });
            }
            return ReturnResult.Fail("access denine");
        }


    }
}
