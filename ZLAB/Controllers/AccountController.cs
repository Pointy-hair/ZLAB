using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Zlab.DataCore;
using Zlab.UtilsCore;
using Zlab.Web.Main.Model.Dtos;
using Zlab.Web.Main.Model.Models;
using Zlab.Web.Main.Service.Interfaces;

namespace ZLAB.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;
        public AccountController()
        {

        }
        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        public async Task<HttpResponseMessage> SendEmail(string email) 
        {
            try
            {
                
                var sent = await accountService.SendEmailAsync(email); 
                return sent ? ReturnMessage.Success() : ReturnMessage.Fail();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ReturnMessage.Fail();
            }
        }

        public async Task<HttpResponseMessage> SignupAsync(SignupModel model) 
        { 
            try
            {
                var dto = await accountService.SignUpAsync(model);
                return ReturnMessage.Success(dto);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ReturnMessage.Fail();
            } 
        }
        public async Task<HttpResponseMessage> Signin(SignupModel model)
        {
            try
            {
                var dto = await accountService.SignUpAsync(model);
                return ReturnMessage.Success(dto);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ReturnMessage.Fail();
            }
        }
    }
}
