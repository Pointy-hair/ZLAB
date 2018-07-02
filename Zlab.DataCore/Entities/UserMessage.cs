using System;
using System.Collections.Generic;
using System.Text;
using Zlab.DataCore.DbCore;

namespace Zlab.DataCore.Entities
{
    public class UserMessage :Entity
    {
        public string MessageId { get; set; }
        public bool Read { get; set; }
        public string UserId { get; set; }
        public string FromUserId { get; set; }
    }
    public class ChannelMessage : Entity
    {
        public string MessageId { get; set; } 
        public string ChannelId { get; set; }
        public string FromUserId { get; set; }
    }
}
