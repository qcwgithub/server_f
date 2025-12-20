using StackExchange.Redis;

namespace Script
{
    public class UserServiceInfoRedis : ServerScript
    {
        public UserServiceInfoRedis(Server server) : base(server)
        {
        }

        public IDatabase GetDb()
        {
            return this.server.data.redis_db;
        }

        
    }
}