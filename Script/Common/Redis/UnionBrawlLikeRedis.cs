using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Data;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Numerics;
using longid = System.Int64;
using System.Linq;

namespace Script
{
    public class UnionBrawlLikeRedis : WaitInitDataRedis<GroupServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }
        protected override string waitKey => GGlobalKey.UnionBrawlInitedFlag();

        string Key(int season) => UnionBrawlKey.Like(season);

        public async Task InitLike(int season, int like)
        {
            await GetDb().StringSetAsync(Key(season), like);
        }

        public async Task<int> IncreaseLike(int season)
        {
            return (int)(await GetDb().StringIncrementAsync(Key(season)));
        }

        public async Task<int> GetLike(int season)
        {
            string s = await GetDb().StringGetAsync(Key(season));
            return RedisUtils.ParseInt(s);
        }
    }
}