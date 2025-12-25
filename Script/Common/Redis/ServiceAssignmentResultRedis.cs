using StackExchange.Redis;

namespace Script
{
    public class ServiceAssignmentResultRedis : ServerScript
    {
        public delegate string GetKeyFunc(long targetId);

        public readonly GetKeyFunc getKeyFunc;
        public ServiceAssignmentResultRedis(Server server, GetKeyFunc getKeyFunc) : base(server)
        {
            this.getKeyFunc = getKeyFunc;
        }

        public IDatabase GetDb()
        {
            return this.server.data.redis_db;
        }

        public async Task SetOwningServiceId(long targetId, int owningServiceId, int secondsToLive)
        {
            string key = this.getKeyFunc(targetId);
            await GetDb().StringSetAsync(key, owningServiceId, TimeSpan.FromSeconds(secondsToLive));
        }

        public async Task<int> GetOwningServiceId(long targetId)
        {
            string key = this.getKeyFunc(targetId);
            RedisValue redisValue = await GetDb().StringGetAsync(key);
            return RedisUtils.ParseInt(redisValue);
        }
    }
}