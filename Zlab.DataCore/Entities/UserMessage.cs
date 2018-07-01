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
    }
}
