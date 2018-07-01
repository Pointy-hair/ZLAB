using System;
using System.Collections.Generic;
using System.Text;
using Zlab.DataCore.DbCore;

namespace Zlab.DataCore.Entities
{
    public class Channel : Entity
    {
        public string ChannelName { get; set; } 
        public IList<string> UserIds { get; set; }
    }
}
