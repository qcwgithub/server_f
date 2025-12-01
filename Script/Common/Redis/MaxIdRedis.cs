using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using Data;
using System;
using longid = System.Int64;

namespace Script
{
    public abstract class MaxIdRedis : WaitInitDataRedis_ServerId<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        public abstract string Key(int serverId);

        public async Task<longid> GetMaxId(int serverId)
        {
            RedisValue redisValue = await GetDb().StringGetAsync(Key(serverId));
            longid id = RedisUtils.ParseLongId(redisValue);
            return id;
        }
        public async Task<bool> SetMaxId(int serverId, longid maxId)
        {
            longidext.CheckLongId(maxId);
            // TODO ALWAYS?
            return await GetDb().StringSetAsync(Key(serverId), maxId, null);
        }

        public virtual async Task<longid> AllocId(int serverId)
        {
            longid id = (await GetDb().StringIncrementAsync(Key(serverId), 1)).i_am_sure_this_is_ok();
            return longidext.CheckLongId(id);
        }
    }
}