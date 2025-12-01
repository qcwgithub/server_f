using System;
using System.Threading.Tasks;
using StackExchange.Redis;
using longid = System.Int64;

namespace Script
{
    public class PlayerOrderRedis : ServerScript<NormalServer>
    {
        public IDatabase GetDb()
        {
            return this.server.serverData.redis_db;
        }

        public string Key(longid playerId, string configId) => PlayerKey.OrderConfigIdTime(playerId, configId);

        public static int EXPIRE_S => 600;

        public async Task SetOrderConfigIdTime(longid playerId, string configId, int timeS)
        {
            await this.GetDb().StringSetAsync(Key(playerId, configId), timeS, TimeSpan.FromSeconds(EXPIRE_S));
        }

        public async Task<int> GetOrderConfigIdTime(longid playerId, string configId)
        {
            RedisValue redisValue = await this.GetDb().StringGetAsync(Key(playerId, configId));
            return RedisUtils.ParseInt(redisValue);
        }

        public async Task DeleteOrderConfigIdTime(longid playerId, string configId, NormalService service)
        {
            bool deleted = await this.GetDb().KeyDeleteAsync(Key(playerId, configId));
            service.logger.InfoFormat("DeleteOrderConfigIdTime: playerId {0} configId {1} deleted? {2}", playerId, configId, deleted);
        }
    }
}