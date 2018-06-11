using StackExchange.Redis;
namespace Zlab.DataCore.DbCore
{
    public class RedisCore
    {
        private static readonly string ConnectionString = "127.0.0.1:6379,password=,connectTimeout=1000,connectRetry=1,syncTimeout=10000";
        private static readonly object locker = new object();
        private static ConnectionMultiplexer instance;
        public static ConnectionMultiplexer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                            instance = GetManager();
                    }
                }
                return instance; 
            }
        }

        private static ConnectionMultiplexer GetManager(string constr = null)
        {
            
            var connect = ConnectionMultiplexer.Connect(ConnectionString);
            
            return connect;
        }

        public static IDatabase GetClient()
        {
            return Instance.GetDatabase(0);
        }
    }
}
