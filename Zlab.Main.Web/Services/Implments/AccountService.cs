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
        public AccountService() { }
        public AccountService(IIdsService idsService, ISessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
            this.idsService = idsService;
        }

        public async Task<string> SignUpAsync(SignupModel model)
        {
            if (model.password.Length < 6)
                return ReturnResult.Fail("password too simple");
            var redis = RedisCore.GetClient();
            if (await redis.StringGetAsync("emailcode:" + model.email) != model.code)
                return ReturnResult.Fail("code error");
            var user = new User
            {
                UserName = $"{await idsService.GetIdAsync("UserId")}#{model.username}",
                Emails = new List<string>() { model.email },
                CreateTime = TimeHelper.GetUnixTimeMilliseconds(),
                Passwords = model.password,
            };
            var repo = new MongoCore<User>();
            var count = await repo.Collection.CountAsync(Builders<User>.Filter.Eq(x => x.Emails.First(), model.email));
            if (count > 0)
                return ReturnResult.Fail("email already registed");
            await repo.Collection.InsertOneAsync(user);
            return ReturnResult.Success(user.UserName);
        }

        public async Task<bool> SendEmailAsync(string email)
        {
            var code = GuidHelper.GetGuid().Substring(0, 4);
            var emailbody = $"your registry code: {code}, please use it in 10 mimutes. this is a system email, do not reply. thank you!";
            var sent = await EmailHelper.SendEmailAsync(email, emailbody, false);
            if (sent)
            {
                var redis = RedisCore.GetClient();
                redis.StringSet("emailcode:", code, TimeSpan.FromSeconds(10));
            }
            return sent;
        }

        public async Task<string> SigninAsync(SigninModel model)
        {
            var filter = Builders<User>.Filter;
            var filters = filter.Eq(x => x.Passwords, model.password) &
                (filter.AnyEq(x => x.Emails, model.username)
                | filter.Eq(x => x.Phone, model.username)
                | filter.Eq(x => x.UserName, model.username));
            var repo = new MongoCore<User>();
            var user = await repo.Collection.Find(filters).Project(x => new { x.Id }).FirstOrDefaultAsync();
            if (user != null)
            {

                if (!string.IsNullOrEmpty(model.device))
                    await sessionManager.AddDeviceAsync(user.Id, model.devicemodel, model.devicename);
                var token = await sessionManager.ReCacheSessionAsync(user.Id);
                if (!string.IsNullOrEmpty(token))
                    return ReturnResult.Success(token);
            }
            return ReturnResult.Fail("access denine");
        }

        
    }
}
