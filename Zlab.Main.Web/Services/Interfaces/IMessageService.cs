using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zlab.Main.Web.Hubs;
using Zlab.Main.Web.Models;

namespace Zlab.Main.Web.Services.Interfaces
{
    public interface IMessageService
    {
        Task<string> SendMessageAsync(SendMsgModel model, IHubContext<SocketHub> hubContext);
        Task<string> GetMessagesAsync(string[] msgid);
        Task<string> GetWebSocketUrlAsync(UserTokenDto model);
    }
}
