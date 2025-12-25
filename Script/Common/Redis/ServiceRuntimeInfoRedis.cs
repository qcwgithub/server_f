using Data;
using StackExchange.Redis;

namespace Script
{
    public class ServiceRuntimeInfoRedis : ServerScript
    {
        public readonly string key;
        public ServiceRuntimeInfoRedis(Server server, string key) : base(server)
        {
            this.key = key;
        }

        public IDatabase GetDb()
        {
            return this.server.data.redis_db;
        }

        public async Task Update(ServiceRuntimeInfo runtimeInfo)
        {
            await this.GetDb().HashSetAsync(this.key, new RedisValue(runtimeInfo.serviceId.ToString()), JsonUtils.stringify(runtimeInfo));
        }

        public async Task<Dictionary<int, ServiceRuntimeInfo>> GetAll()
        {
            HashEntry[] entries = await this.GetDb().HashGetAllAsync(this.key);
            var dict = new Dictionary<int, ServiceRuntimeInfo>();
            foreach (HashEntry entry in entries)
            {
                int serviceId = int.Parse(entry.Name.ToString());
                var info = JsonUtils.parse<ServiceRuntimeInfo>(entry.Value.ToString());
                dict[serviceId] = info;
            }
            return dict;
        }
    }
}