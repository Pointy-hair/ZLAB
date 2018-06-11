using Microsoft.AspNetCore.Identity.MongoDB;
using System;
using MongoDB.Driver;
using Zlab.DataCore.DbCore;

namespace Zlab.DataCore.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public long CreateTime { get; set; }

    }

    public class ApplicationUserRepository : IDisposable
    {
        public ApplicationUserRepository()
        {
            Collection = MongoDbHelper.GetDatabase().GetCollection<ApplicationUser>("user");
        }

        /// <summary>
        /// mongo collection
        /// </summary>
        public IMongoCollection<ApplicationUser> Collection { get; private set; }

        public void Dispose()
        {
        }
    }
}
