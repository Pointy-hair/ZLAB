﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zlab.Main.Web.Hubs
{
    public class PushMessage
    {
        public PushType type { get; set; }
        public List<string> msgids { get; set; }
        public string RtcBody { get; set; } 
        public string invite { get; set; }

    }
    public enum PushType
    {
        MessageId=0,
        MessageBack=1,
        ChannelInvite = 2,
        Rtc =10, 
        HearBeat=100,
       
    }
}
