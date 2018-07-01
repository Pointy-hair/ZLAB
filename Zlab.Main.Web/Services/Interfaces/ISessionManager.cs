using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zlab.Main.Web.Services.Interfaces
{
    public  interface ISessionManager
    {
        Task<string> GetUserIdAsync(string token);
        Task<string> ReCacheSessionAsync(string userid);
        Task<bool> AddDeviceAsync(string userid, string deviceModel, string deviceName);
        Task<bool> RemoveDeviceAsync(string userid, string deviceModel, string deviceName);
        Task<string> GetSocketUserIdAsync(string token);
        Task<string> ReCacheSocketSessionAsync(string userid);
        Task<bool> CheckeUserAsync(string userid, string token);
    }
}
