using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Zlab.DataCore;
using Zlab.Main.Web.Models;
using Zlab.Main.Web.Service.Interfaces;
using Zlab.UtilsCore;

using System.Collections.Generic;

namespace ZLAB.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;
       // public AccountController(){}
         
        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }
        [HttpGet,Route("email/code")]
        public async Task<string> SendEmail([FromQuery]string email) 
        {
            try
            {
                
                var sent = await accountService.SendEmailAsync(email); 
                var msg= sent ? ReturnResult.Success() : ReturnResult.Fail(); 
                return msg;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ReturnResult.Fail();
            }
        }
        [HttpPost,Route("signup")]
        public async Task<string> SignupAsync([FromBody]SignupModel model) 
        { 
            try
            {
                var content = await accountService.SignUpAsync(model);
                return content;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ReturnResult.Fail(ex);
            } 
        }
        [HttpPost, Route("signin")]
        public async Task<string> Signin([FromBody]SigninModel model)
        {
            try
            {
                var content = await accountService.SigninAsync(model);
                return content;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ReturnResult.Fail();
            }
        }
        [HttpGet]
        public string Get()
        {
            return ReturnResult.Success();
        }
    }
}
