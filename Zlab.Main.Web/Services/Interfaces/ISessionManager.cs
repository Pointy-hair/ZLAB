using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zlab.Main.Web.Services.Interfaces
{
    public  interface ISessionManager
    {
        Task<String> GetSessionAsync(string userid);
        Task<string> ReCacheSessionAsync(string userid);
        Task<bool> AddDeviceAsync(string userid, string device);
        Task<bool> RemoveDeviceAsync(string userid, string device);
    }
}
