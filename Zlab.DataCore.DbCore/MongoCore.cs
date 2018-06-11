using MongoDB.Driver;


namespace Zlab.DataCore.DbCore
{
    public class MongoCore<T>
    {
        public IMongoCollection<T> Collection { get; private set; }
        public MongoCore()
        {
            var db = MongoDbHelper.GetDatabase();
            Collection = db.GetCollection<T>(typeof(T).Name);
        }
        
    }
    public class MongoDbHelper
    {
        public static IMongoDatabase GetDatabase()
        {
            var setting = new MongoClientSettings()
            {
                //ConnectTimeout = TimeSpan.FromSeconds(60),
                Server = new MongoServerAddress("127.0.0.1", 27017),

            };
            var client = new MongoClient(setting);
            return client.GetDatabase("zlab");
        } 
    }

}
