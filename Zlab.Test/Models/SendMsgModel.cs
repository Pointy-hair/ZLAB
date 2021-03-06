﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Zlab.Test.Models
{
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
}
