using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlab.UWP.View.Models
{
    public class UserTokenDto
    {
        public string userid { get; set; }
        public string token { get; set; }
    }
    public class SendMsgModel
    {
        public List<string> touserids { get; set; }
        public string message { get; set; }
        public MessageType type { get; set; }
        public List<string> imgs { get; set; }
        public List<string> ats { get; set; }
        public MessageGroup group { get; set; }
        public string token { get; set; }
    }
    public enum MessageType
    {
        text = 0,
        audio = 1,
        video = 2,
        rtc = 3,
        share = 4
    }
    public enum MessageGroup
    {
        Personal = 0,
        Channel = 1,
        Notify = 2,

    }
    public class SignupModel
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string code { get; set; }
    }
    public class SigninModel
    {
        public string username { get; set; }
        public string password { get; set; }
        //public string devicemodel { get; set; }
        //public string devicename { get; set; }
    }
}
