using Microsoft.AspNetCore.Identity.MongoDB;
using System;
using MongoDB.Driver;
using Zlab.DataCore.DbCore;
using System.Collections.Generic;

namespace Zlab.DataCore.Entities
{
    public class User : Entity
    { 
        public long CreateTime { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public IList<string> Emails { get; set; }
        public string Phone { get; set; } 
        public string CountryCode { get; set; } 
        public IList<string> ChannelIds { get; set; } 
    } 
     
}
