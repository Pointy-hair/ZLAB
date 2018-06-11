using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zlab.DataCore.Entities;

namespace Zlab.Web.Main.Configs
{
    public class IdentityConfig
    {
         
    }

    //public class ApplicationUserManager : UserManager<ApplicationUser>
    //{
    //    public IMongoCollection<ApplicationUser> UserCollection;
    //    public ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger)
    //      : base(store, optionsAccessor,passwordHasher,userValidators,passwordValidators,keyNormalizer,errors,services,logger)
    //    {
    //    }
    //    public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
    //        IOwinContext context)
    //    {
    //        IMongoCollection<ApplicationUser> userCollection = context.Get<ApplicationIdentityContext>().Users;

    //        var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(userCollection));

    //        manager.UserCollection = userCollection;

    //        // Configure validation logic for usernames
           

    //        // Configure user lockout defaults
    //        //manager.UserLockoutEnabledByDefault = true;
    //        //manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
    //        //manager.MaxFailedAccessAttemptsBeforeLockout = 5;

    //        // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
    //        // You can write your own provider and plug it in here.
    //        //manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
    //        //{
    //        //    MessageFormat = "Your security code is {0}"
    //        //});
    //        //manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
    //        //{
    //        //    Subject = "Security Code",
    //        //    BodyFormat = "Your security code is {0}"
    //        //});
    //        //manager.EmailService = new EmailService();
    //        //manager.SmsService = new SmsService();
    //        var dataProtectionProvider = options.DataProtectionProvider;
    //        if (dataProtectionProvider != null)
    //        {
    //            manager.UserTokenProvider =
    //                new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
    //        }
    //        return manager;
    //    }
    //}
}
