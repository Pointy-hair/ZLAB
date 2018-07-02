using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zlab.Main.Web.Models
{
    public class AddChannelModel : UserToken
    {
        public string channelid { get; set; }
        public string invatetoken { get; set; }

    }
    public class InviteChannelModel : UserToken
    {
        public string[] inviteuserids { get; set; }
        public string channelid { get; internal set; }
    }
    public class RemoveChannelModel : UserToken
    {
        public string[] removeuserids { get; set; }
        public string channelid { get; set; }
        public string moveuserid { get; set; }
    }
}
