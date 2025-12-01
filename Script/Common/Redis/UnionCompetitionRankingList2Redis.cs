using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Data;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Numerics;
using longid = System.Int64;

namespace Script
{
    // 联盟战：赛季积分排行榜
    public class UnionCompetitionRankingList2Redis : GWaitInitDataRedis<GroupServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        protected override string waitKey => GGlobalKey.UnionCompetitionV2InitedFlag();

        protected string Key()
        {
            return UnionCKey.RankingList2();
        }

        public async Task SetNewScore(longid unionId, int score, int timeS)
        {
            await GetDb().SortedSetAddAsync(this.Key(), unionId, TimeUtils.AddTimeFactor(score, timeS));
        }

        public async Task<long> Length()
        {
            return await GetDb().SortedSetLengthAsync(this.Key());
        }

        public async Task DeleteAll()
        {
            await this.GetDb().KeyDeleteAsync(this.Key());
        }

        public async Task DeleteOne(longid unionId)
        {
            await GetDb().SortedSetRemoveAsync(this.Key(), unionId);
        }

        // 顺序即为排名
        public async Task<List<longid>> GetAll()
        {
            RedisValue[] redisValues = await GetDb().SortedSetRangeByScoreAsync(this.Key(), double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Descending);
            return redisValues.Select(_ => RedisUtils.ParseLongId(_)).ToList();
        }
    }
}