﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zlab.Main.Web.Services.Interfaces
{
    public interface IMessageService
    {
        Task<bool> SendMessageAsync(List<string> touserids, string message);
    }
}
