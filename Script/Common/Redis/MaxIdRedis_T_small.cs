using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using Data;
using System;

namespace Script
{
    // small 指不是 N*100亿 + X 的格式，而是只有 X
    public abstract class MaxIdRedis_small<T> : WaitInitDataRedis<NormalServer>
    {
        public readonly T t;
        public MaxIdRedis_small(T t)
        {
            this.t = t;
        }

        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        public abstract string Key();

        public async Task<long> GetMaxId()
        {
            RedisValue redisValue = await GetDb().StringGetAsync(Key());
            long id = RedisUtils.ParseLong(redisValue);
            return id;
        }

        protected async Task<bool> SetMaxId(long maxId)
        {
            // TODO ALWAYS?
            return await GetDb().StringSetAsync(Key(), maxId, null);
        }

        public virtual async Task<long> AllocId()
        {
            long id = await GetDb().StringIncrementAsync(Key(), 1);
            return id;
        }
    }
}