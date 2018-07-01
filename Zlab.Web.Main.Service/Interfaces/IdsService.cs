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
                    IsUpsert = true
                });
            return (id?.Value ?? 0);
        }
        public async Task SetIdAsync(string key, int value)
        {
            var repo = new MongoCore<Ids>();
            var filter = Builders<Ids>.Filter.Eq(x => x.Key, key);
            var update = Builders<Ids>.Update.Set(x => x.Value, value);
            if (await repo.Collection.CountAsync(filter) < 1)
                await repo.Collection.UpdateOneAsync(filter, update,new UpdateOptions
                {
                    IsUpsert=true, 
                });
        }
    }
}
