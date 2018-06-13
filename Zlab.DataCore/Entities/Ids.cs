using System;
using System.Collections.Generic;
using System.Text;
using Zlab.DataCore.DbCore;

namespace Zlab.DataCore.Entities
{
    public class Ids : Entity
    {
        public string Key { get; set; }
        public int Value { get; set; }
    }
}
