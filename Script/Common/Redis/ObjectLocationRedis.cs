using Data;
using StackExchange.Redis;

namespace Script
{
    public abstract class ObjectLocationRedis : ServerScript
    {
        public delegate string GetKeyFunc(long targetId);

        public readonly GetKeyFunc getKeyFunc;
        public ObjectLocationRedis(Server server, GetKeyFunc getKeyFunc) : base(server)
        {
            this.getKeyFunc = getKeyFunc;
        }

        public IDatabase GetDb()
        {
            return this.server.data.redis_db;
        }

        public async Task WriteLocation(long objectId, int serviceId, int secondsToLive)
        {
            string key = this.getKeyFunc(objectId);
            long expiry = TimeUtils.GetTimeS() + secondsToLive;
            string value = $"{serviceId},{expiry}";
            await GetDb().StringSetAsync(key, value, TimeSpan.FromSeconds(secondsToLive));
        }
    }

    public class ObjectLocationRedisW : ObjectLocationRedis
    {
        public ObjectLocationRedisW(Server server, GetKeyFunc getKeyFunc) : base(server, getKeyFunc)
        {
        }
    }

    public class ObjectLocationRedisRW : ObjectLocationRedis
    {
        public ObjectLocationRedisRW(Server server, GetKeyFunc getKeyFunc) : base(server, getKeyFunc)
        {
        }

        public async Task<stObjectLocation> GetLocation(long objectId)
        {
            string key = this.getKeyFunc(objectId);
            RedisValue redisValue = await GetDb().StringGetAsync(key);
            if (redisValue.IsNullOrEmpty)
            {
                return default;
            }
            string s = redisValue.ToString();
            int dot = s.IndexOf(',');
            MyDebug.Assert(dot > 0);

            int serviceId = int.Parse(s.Substring(0, dot));
            long expiry = long.Parse(s.Substring(dot + 1));
            return new stObjectLocation { serviceId = serviceId, expiry = expiry };
        }
    }
}