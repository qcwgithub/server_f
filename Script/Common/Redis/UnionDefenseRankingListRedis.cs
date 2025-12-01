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
    public class UnionDefenseRankingListRedis : WaitInitDataRedis_ServerId<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        public override string WaitKey(int serverId) => GlobalKey.UnionDefenseInitedFlag(serverId);

        protected string Key(stParentServerId parentServerId)
        {
            return UnionDefenseKey.RankingList(parentServerId);
        }

        public async Task<(RedisKey, bool)> Clear(stParentServerId parentServerId)
        {
            RedisKey key = this.Key(parentServerId);
            return (key, await this.GetDb().KeyDeleteAsync(key));
        }

        public async Task SetNewScore(stParentServerId parentServerId, longid unionId, int score, int timeS)
        {
            await GetDb().SortedSetAddAsync(this.Key(parentServerId), unionId, TimeUtils.AddTimeFactor(score, timeS));
        }

        public async Task<long> Length(stParentServerId parentServerId)
        {
            return await GetDb().SortedSetLengthAsync(this.Key(parentServerId));
        }

        public async Task DeleteAll(stParentServerId parentServerId)
        {
            await this.GetDb().KeyDeleteAsync(this.Key(parentServerId));
        }

        public async Task DeleteOne(stParentServerId parentServerId, longid unionId)
        {
            await GetDb().SortedSetRemoveAsync(this.Key(parentServerId), unionId);
        }

        // 顺序即为排名
        public async Task<List<longid>> GetAll(stParentServerId parentServerId)
        {
            RedisValue[] redisValues = await GetDb().SortedSetRangeByScoreAsync(this.Key(parentServerId), double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Descending);
            return redisValues.Select(_ => RedisUtils.ParseLongId(_)).ToList();
        }
    }
}