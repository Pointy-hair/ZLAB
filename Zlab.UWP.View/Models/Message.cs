using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zlab.UWP.View.Models
{
    public class Message
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
