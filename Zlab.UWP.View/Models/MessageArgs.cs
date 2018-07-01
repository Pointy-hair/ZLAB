using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlab.UWP.View.Models
{
    public class MessageArgs :EventArgs
    {
        public PushMessage Messsage { get; set; } 
    }

    public class PushMessage
    {
        public PushType type { get; set; }
        public List<string> msgids { get; set; }
        public string RtcBody { get; set; }

    }
    public enum PushType
    {
        MessageId = 0,
        MessageBack = 1,
        Rtc = 10,
        HearBeat = 100,
    }
}
