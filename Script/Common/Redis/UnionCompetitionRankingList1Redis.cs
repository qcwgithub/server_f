using System.Linq;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Data;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Numerics;
using longid = System.Int64;

namespace Script
{
    // 联盟战：前10个率先打完全部地区的联盟
    public class UnionCompetitionRankingList1Redis : GWaitInitDataRedis<GroupServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        protected override string waitKey => GGlobalKey.UnionCompetitionV2InitedFlag();

        protected string Key()
        {
            return UnionCKey.RankingList1();
        }

        public async Task Add(longid unionId, int timeS)
        {
            await GetDb().SortedSetAddAsync(this.Key(), unionId, TimeUtils.AddTimeFactor(0, timeS));
        }

        public async Task<long> Length()
        {
            return await GetDb().SortedSetLengthAsync(this.Key());
        }

        public async Task<List<longid>> GetFirst10()
        {
            RedisValue[] redisValues = await GetDb().SortedSetRangeByRankAsync(this.Key(), 0, 9, Order.Descending);
            return redisValues.Select(_ => RedisUtils.ParseLongId(_)).ToList();
        }

        public async Task DeleteAll()
        {
            await this.GetDb().KeyDeleteAsync(this.Key());
        }
    }
}