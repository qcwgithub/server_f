using Data;
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

        public async Task<Dictionary<int, UserServiceInfo>> GetAll()
        {
            HashEntry[] entries = await this.GetDb().HashGetAllAsync(CommonKey.UserServiceInfos());
            var dict = new Dictionary<int, UserServiceInfo>();
            foreach (HashEntry entry in entries)
            {
                int serviceId = int.Parse(entry.Name.ToString());
                var info = JsonUtils.parse<UserServiceInfo>(entry.Value.ToString());
                dict[serviceId] = info;
            }
            return dict;
        }
    }
}