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
    public abstract class BaseExpeditionPartyRankingListRedis : WaitInitDataRedis<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        protected override string waitKey => GlobalKey.ExpeditionPartyInitedFlag();

        public abstract string Key(int groupId);

        public async Task<int> GetScore(int groupId, longid playerId)
        {
            double? r = await GetDb().SortedSetScoreAsync(this.Key(groupId), playerId);
            int result = (r == null ? 0 : (int)TimeUtils.RemoveTimeFactor(r.Value));
            return result;
        }

        public async Task<(RedisKey, bool)> Clear(int groupId)
        {
            RedisKey key = this.Key(groupId);
            return (key, await this.GetDb().KeyDeleteAsync(key));
        }

        public async Task SetNewScore(int groupId, longid playerId, BigInteger newScore, int timeS, log4net.ILog logger)
        {
            await GetDb().SortedSetAddAsync(this.Key(groupId), playerId, TimeUtils.AddTimeFactor(RankUtils.BigIntegerToDoubleSafe(newScore, logger), timeS));
        }

        public async Task SetNewScoreMany(int groupId, (longid, BigInteger, int)[] playerId_newScore_timeS, log4net.ILog logger)
        {
            SortedSetEntry[] entries = playerId_newScore_timeS.Select(x => new SortedSetEntry(x.Item1, TimeUtils.AddTimeFactor(RankUtils.BigIntegerToDoubleSafe(x.Item2, logger), x.Item3))).ToArray();
            await GetDb().SortedSetAddAsync(this.Key(groupId), entries);
        }

        public async Task<long> GetLength(int groupId)
        {
            return await GetDb().SortedSetLengthAsync(this.Key(groupId));
        }

        public async Task<int> GetRank(int groupId, longid playerId)
        {
            long? r = await GetDb().SortedSetRankAsync(this.Key(groupId), playerId, Order.Descending);
            if (r == null)
            {
                return RankUtils.INVALID_RANK;
            }
            else
            {
                return RankUtils.FromIndex((int)r.Value);
            }
        }

        public async Task GetAll(int groupId, /*OUT*/ List<longid> playerIds)
        {
            RedisValue[] redisValues = await GetDb().SortedSetRangeByScoreAsync(this.Key(groupId), double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Descending);
            if (redisValues != null)
            {
                foreach (RedisValue v in redisValues)
                {
                    playerIds.Add(RedisUtils.ParseLongId(v));
                }
            }
        }

        public async Task GetByIndexRange(int groupId, int startIndex, int count, /*OUT*/List<longid> playerIds)
        {
            int endIndex = count == -1 ? -1 : startIndex + count - 1;

            RedisValue[] redisValues = await GetDb().SortedSetRangeByRankAsync(this.Key(groupId), startIndex, endIndex, Order.Descending);
            if (redisValues != null)
            {
                foreach (RedisValue v in redisValues)
                {
                    playerIds.Add(RedisUtils.ParseLongId(v));
                }
            }
        }

        public async Task DeleteAll(int groupId, log4net.ILog logger)
        {
            logger.InfoFormat("delete key '{0}'", this.Key(groupId));
            await this.GetDb().KeyDeleteAsync(this.Key(groupId));
        }
    }

    public class ExpeditionPartyRankingListRedis : BaseExpeditionPartyRankingListRedis
    {
        public override string Key(int groupId) => ExpeditionPartyKey.RankingList(groupId);
    }

    public class TempExpeditionPartyRankingListRedis : BaseExpeditionPartyRankingListRedis
    {
        public override string Key(int groupId) => TemporaryKey.ToTemporaryKey(ExpeditionPartyKey.RankingList(groupId), "forSumUp");
    }
}