using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zlab.DataCore.Entities;

namespace Zlab.Main.Web.Models
{
    public class UserMessageDto
    {
        public string Id { get; set; }
        public string Sender { get; set; }
        public IList<string> ToUserIds { get; set; }
        public string Body { get; set; }
        public IList<string> ImagePaths { get; set; } 
        public IList<string> AtUserIds { get; set; }
        public MessageType Type { get; set; }
        public MessageGroup Group { get; set; }
        public long CreateTime { get; set; }
    }
}
