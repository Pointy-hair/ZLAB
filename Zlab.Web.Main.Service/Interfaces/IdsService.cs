using System.Threading.Tasks;
using Zlab.DataCore.Entities;
using Zlab.Web.Service.Implments;
using Zlab.DataCore.DbCore;
using MongoDB.Driver;

namespace Zlab.Web.Service.Interfaces
{
    public class IdsService : IIdsService
    {
        public async Task<int> GetIdAsync(string key)
        {
            var repo = new MongoCore<Ids>();
            var id = await repo.Collection.FindOneAndUpdateAsync(Builders<Ids>.Filter.Eq(x => x.Key, key), Builders<Ids>.Update.Inc(x => x.Value, 1),
                new FindOneAndUpdateOptions<Ids, Ids>
                {
                    ReturnDocument = ReturnDocument.After,
                });
            return (id?.Value ?? 0);
        }
    }
}
