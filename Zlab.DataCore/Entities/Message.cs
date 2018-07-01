using System.Collections.Generic;
using Zlab.DataCore.DbCore;

namespace Zlab.DataCore.Entities
{
    public class Message : Entity
    {
        public string Sender { get; set; }
        public IList<string> ToUserIds { get; set; }
        public string Body { get; set; }
        public IList<string> ImagePaths { get; set; }
        public IList<string> AtUserIds { get; set; }
        public MessageType Type { get; set; }
        public MessageGroup Group { get; set; }
        public long CreateTime { get; set; }  
    }
    public enum MessageType
    {
        text = 0,
        audio = 1,
        video = 2, 
        share = 4
    }
    public enum MessageGroup
    {
        Personal=0,
        Channel = 1, 
        Notify=2, 
    }
}
