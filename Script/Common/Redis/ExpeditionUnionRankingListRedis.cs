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
    public abstract class BaseExpeditionUnionRankingListRedis : WaitInitDataRedis_ServerId<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        public override string WaitKey(int serverId) => GlobalKey.ExpeditionInitedFlag(serverId);

        public abstract string Key(stParentServerId parentServerId);

        public async Task<int> GetScore(stParentServerId parentServerId, longid unionId)
        {
            double? r = await GetDb().SortedSetScoreAsync(this.Key(parentServerId), unionId);
            int result = (r == null ? 0 : (int)TimeUtils.RemoveTimeFactor(r.Value));
            return result;
        }

        public async Task<(RedisKey, bool)> Clear(stParentServerId parentServerId)
        {
            RedisKey key = this.Key(parentServerId);
            return (key, await this.GetDb().KeyDeleteAsync(key));
        }

        public async Task SetNewScore(stParentServerId parentServerId, longid unionId, BigInteger newScore, int timeS, log4net.ILog logger)
        {
            await GetDb().SortedSetAddAsync(this.Key(parentServerId), unionId, TimeUtils.AddTimeFactor(RankUtils.BigIntegerToDoubleSafe(newScore, logger), timeS));
        }

        public async Task SetNewScoreMany(stParentServerId parentServerId, (longid, BigInteger, int)[] unionId_newScore_timeS, log4net.ILog logger)
        {
            SortedSetEntry[] entries = unionId_newScore_timeS.Select(x => new SortedSetEntry(x.Item1, TimeUtils.AddTimeFactor(RankUtils.BigIntegerToDoubleSafe(x.Item2, logger), x.Item3))).ToArray();
            await GetDb().SortedSetAddAsync(this.Key(parentServerId), entries);
        }

        public async Task<long> GetLength(stParentServerId parentServerId)
        {
            return await GetDb().SortedSetLengthAsync(this.Key(parentServerId));
        }

        public async Task<int> GetRank(stParentServerId parentServerId, longid unionId)
        {
            long? r = await GetDb().SortedSetRankAsync(this.Key(parentServerId), unionId, Order.Descending);
            if (r == null)
            {
                return RankUtils.INVALID_RANK;
            }
            else
            {
                return RankUtils.FromIndex((int)r.Value);
            }
        }

        public async Task GetAll(stParentServerId parentServerId, /*OUT*/ List<longid> unionIds)
        {
            RedisValue[] redisValues = await GetDb().SortedSetRangeByScoreAsync(this.Key(parentServerId), double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Descending);
            if (redisValues != null)
            {
                foreach (RedisValue v in redisValues)
                {
                    unionIds.Add(RedisUtils.ParseLongId(v));
                }
            }
        }

        public async Task GetByIndexRange(stParentServerId parentServerId, int startIndex, int count, /*OUT*/List<longid> unionIds)
        {
            int endIndex = count == -1 ? -1 : startIndex + count - 1;

            RedisValue[] redisValues = await GetDb().SortedSetRangeByRankAsync(this.Key(parentServerId), startIndex, endIndex, Order.Descending);
            if (redisValues != null)
            {
                foreach (RedisValue v in redisValues)
                {
                    unionIds.Add(RedisUtils.ParseLongId(v));
                }
            }
        }

        public async Task DeleteAll(stParentServerId parentServerId, log4net.ILog logger)
        {
            logger.InfoFormat("delete key '{0}'", this.Key(parentServerId));
            await this.GetDb().KeyDeleteAsync(this.Key(parentServerId));
        }
    }

    public class ExpeditionUnionRankingListRedis : BaseExpeditionUnionRankingListRedis
    {
        public override string Key(stParentServerId parentServerId) => ExpeditionKey.UnionRankingList(parentServerId);
    }

    public class TempExpeditionUnionRankingListRedis : BaseExpeditionUnionRankingListRedis
    {
        public override string Key(stParentServerId parentServerId) => TemporaryKey.ToTemporaryKey(ExpeditionKey.UnionRankingList(parentServerId), "forSumUp");
    }
}