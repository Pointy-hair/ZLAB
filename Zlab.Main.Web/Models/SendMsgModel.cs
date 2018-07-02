using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zlab.DataCore.Entities;

namespace Zlab.Main.Web.Models
{
    public class SendMsgModel:UserToken
    {
        public string touserid { get; set; }
        public string tochannelid { get; set; }  
        public string message { get; set; }
        public MessageType type { get; set; }
        public List<string> imgs { get; set; }
        public List<string> ats { get; set; }
        public MessageGroup group { get; set; } 
    }
}
