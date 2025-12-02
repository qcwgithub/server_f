using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using Data;
using System;


namespace Script
{
    public abstract class GMaxIdRedis : GWaitInitDataRedis<BaseServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        public abstract string Key();
        protected sealed override string waitKey => this.Key();

        public async Task<long> GetMaxId()
        {
            RedisValue redisValue = await GetDb().StringGetAsync(Key());
            return RedisUtils.ParseLongId(redisValue);
        }
        public async Task<bool> SetMaxId(long maxId)
        {
            // TODO ALWAYS?
            return await GetDb().StringSetAsync(Key(), maxId, null);
        }

        public virtual async Task<long> AllocId()
        {
            return (await GetDb().StringIncrementAsync(Key(), 1)).i_am_sure_this_is_ok();
        }
    }
}