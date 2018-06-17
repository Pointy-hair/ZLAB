using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zlab.Main.Web.Models;

namespace Zlab.Main.Web.Services.Interfaces
{
    public interface IMessageService
    {
        Task<bool> SendMessageAsync(SendMsgModel model);
    }
}
