using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using Zlab.DataCore.DbCore;
using Zlab.DataCore.Entities;
using Zlab.UtilsCore;
using Zlab.Web.Main.Model.Dtos;
using Zlab.Web.Main.Model.Models;
using Zlab.Web.Main.Service.Interfaces;

namespace Zlab.Web.Main.Service.Implments
{
    public class AccountService : IAccountService
    {
        UserManager<ApplicationUser> userManager;

        public async Task<SignupDTO> SignUpAsync(SignupModel model)
        {
            var redis = RedisCore.GetClient();
            if (await redis.StringGetAsync("emailcode:" + model.emial) != model.code)
                return new SignupDTO();
            var user = new ApplicationUser
            {

            };
            var created = await userManager.CreateAsync(user);
            return new SignupDTO();
        }

        public async Task<bool> SendEmailAsync(string email)
        {
            var code = GuidHelper.GetGuid().Substring(0, 4);
            var sent = await EmailHelper.SendEmailAsync(email, code, false);
            if (sent)
            {
                var redis = RedisCore.GetClient();
                redis.StringSet("emailcode:", code);
            }
            return sent;
        }
    }
}
