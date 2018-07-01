using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlab.UWP.View.Models
{
    public static class Urls
    {
        public static readonly string UrlPrefix = "http://zlab.yixinin.xyz";
        #region Accounts

        public static readonly string SendEamil = UrlPrefix + "/api/account/email/code";
        public static readonly string Signup = UrlPrefix + "/api/account/signup";
        public static readonly string Longin = UrlPrefix + "/api/account/signin";

        #endregion

        #region Message

        public static readonly string SendMessage = UrlPrefix + "/api/msg/sendmsg";
        public static readonly string GetSocketUrl = UrlPrefix + "/api/msg/socketurl";
        public static readonly string ReadMessage = UrlPrefix + "/api/msg/read"; 
            public static readonly string GetMessage = UrlPrefix + "/api/msg/getmsgs";
        public static readonly string SocketUrl = "http://zlab.yixinin.xyz/websocket";

        #endregion
    }
}
