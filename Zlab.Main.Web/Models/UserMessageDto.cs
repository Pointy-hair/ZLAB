﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zlab.DataCore.Entities;

namespace Zlab.Main.Web.Models
{
    public class UserMessageDto
    {
        public string id { get; set; }
        public string sender { get; set; } 
        public string body { get; set; }
        public IList<string> imagepaths { get; set; } 
        public IList<string> atuserids { get; set; }
        public MessageType type { get; set; }
        public MessageGroup group { get; set; }
        public long createtime { get; set; }
    }
}
