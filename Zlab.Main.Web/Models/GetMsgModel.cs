using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zlab.Main.Web.Models
{
    public class GetMsgModel : UserToken
    {
        public string[] msgids { get; set; }
    }
    public class GetUnReadMsgModel : UserToken
    {
        public string userid { get; set; }
    }
}
