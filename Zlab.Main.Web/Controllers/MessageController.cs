
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Zlab.DataCore;
using Zlab.Main.Web.Hubs;
using Zlab.Main.Web.Models;
using Zlab.Main.Web.Services.Interfaces;
using Zlab.UtilsCore;

namespace Zlab.Web.Main.Services.Interfaces
{
    [Route("api/msg")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService messageService;
        private readonly ISessionManager sessionManager;
        // public MessageController() { }
        private readonly IHubContext<SocketHub> hubContext;
        public MessageController(IMessageService messageService, IHubContext<SocketHub> hubContext, ISessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
            this.messageService = messageService;
            this.hubContext = hubContext;
        }
        [HttpPost, Route("sendmsg")]
        public async Task<string> SendMessage([FromBody]SendMsgModel model)
        {
            try
            {
                if (await sessionManager.CheckUserAsync(model.userid, model.token))
                    return ReturnResult.Fail("access denied");
                var content = await messageService.SendMessageAsync(model, hubContext);
                return content;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ReturnResult.Fail(ex);
            }

        }
        [HttpPost, Route("getmsgs")]
        public async Task<string> GetMessage([FromBody]GetMsgModel model)
        {
            try
            {
                if (await sessionManager.CheckUserAsync(model.userid, model.token))
                    return ReturnResult.Fail("access denied");
                var content = await messageService.GetMessagesAsync(model.msgids);
                return content;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
            return ReturnResult.Fail();
        }

        [HttpGet, Route("getmsgs")]
        public async Task<string> GetMessage([FromQuery] UserToken model)
        {
            try
            {
                if (await sessionManager.CheckUserAsync(model.userid, model.token))
                    return ReturnResult.Fail("access denied");
                var content = await messageService.GetMessagesAsync(model.userid);
                return content;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
            return ReturnResult.Fail();
        }
        [HttpGet, Route("socketurl")]
        public async Task<string> GetWebSockteUrl([FromQuery]UserTokenDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model?.token))
                    return ReturnResult.Fail("null");
                var content = await messageService.GetWebSocketUrlAsync(model);
                return content;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                return ReturnResult.Fail(ex);
            }
        } 
    }
}
