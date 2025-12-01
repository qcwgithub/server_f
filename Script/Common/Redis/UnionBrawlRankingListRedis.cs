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
    public abstract class BaseUnionBrawlRankingListRedis : WaitInitDataRedis<GroupServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        protected override string waitKey => GGlobalKey.UnionBrawlInitedFlag();

        public abstract string Key(int groupId);

        public async Task<int> GetScore(int groupId, longid playerOrUnionId)
        {
            double? r = await GetDb().SortedSetScoreAsync(this.Key(groupId), playerOrUnionId);
            int result = (r == null ? 0 : (int)TimeUtils.RemoveTimeFactor(r.Value));
            return result;
        }

        // 开战初始化 ✓
        // GGlobal 初始化 ✓
        public async Task SetNewScore(int groupId, longid playerOrUnionId, BigInteger newScore, int timeS, log4net.ILog logger)
        {
            await GetDb().SortedSetAddAsync(this.Key(groupId), playerOrUnionId, TimeUtils.AddTimeFactor(RankUtils.BigIntegerToDoubleSafe(newScore, logger), timeS));
        }

        // GGlobal 初始化 ✓
        public async Task SetNewScoreMany(int groupId, (longid, BigInteger, int)[] playerOrUnionId_newScore_timeS, log4net.ILog logger)
        {
            SortedSetEntry[] entries = playerOrUnionId_newScore_timeS.Select(x => new SortedSetEntry(x.Item1, TimeUtils.AddTimeFactor(RankUtils.BigIntegerToDoubleSafe(x.Item2, logger), x.Item3))).ToArray();
            await GetDb().SortedSetAddAsync(this.Key(groupId), entries);
        }

        public async Task<long> GetLength(int groupId)
        {
            return await GetDb().SortedSetLengthAsync(this.Key(groupId));
        }

        public async Task<int> GetRank(int groupId, longid playerOrUnionId)
        {
            long? r = await GetDb().SortedSetRankAsync(this.Key(groupId), playerOrUnionId, Order.Descending);
            if (r == null)
            {
                return RankUtils.INVALID_RANK;
            }
            else
            {
                return RankUtils.FromIndex((int)r.Value);
            }
        }

        public async Task GetAll(int groupId, /*OUT*/ List<longid> playerOrUnionIds)
        {
            RedisValue[] redisValues = await GetDb().SortedSetRangeByScoreAsync(this.Key(groupId), double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Descending);
            if (redisValues != null)
            {
                foreach (RedisValue v in redisValues)
                {
                    playerOrUnionIds.Add(RedisUtils.ParseLongId(v));
                }
            }
        }

        public async Task GetByIndexRange(int groupId, int startIndex, int count, /*OUT*/List<longid> playerOrUnionIds)
        {
            int endIndex = count == -1 ? -1 : startIndex + count - 1;

            RedisValue[] redisValues = await GetDb().SortedSetRangeByRankAsync(this.Key(groupId), startIndex, endIndex, Order.Descending);
            if (redisValues != null)
            {
                foreach (RedisValue v in redisValues)
                {
                    playerOrUnionIds.Add(RedisUtils.ParseLongId(v));
                }
            }
        }

        public async Task DeleteAll(int groupId, log4net.ILog logger)
        {
            logger.InfoFormat("delete key '{0}'", this.Key(groupId));
            await this.GetDb().KeyDeleteAsync(this.Key(groupId));
        }
    }

    public class UnionBrawl_PlayerRankingListRedis : BaseUnionBrawlRankingListRedis
    {
        public override string Key(int groupId) => UnionBrawlKey.PlayerRankingList(groupId);
    }

    public class SumupUnionBrawl_PlayerRankingListRedis : BaseUnionBrawlRankingListRedis
    {
        public override string Key(int groupId) => TemporaryKey.ToTemporaryKey(UnionBrawlKey.PlayerRankingList(groupId), "forSumUp");
    }

    public class UnionBrawl_UnionRankingListRedis : BaseUnionBrawlRankingListRedis
    {
        public override string Key(int groupId) => UnionBrawlKey.UnionRankingList(groupId);
    }

    public class SumupUnionBrawl_UnionRankingListRedis : BaseUnionBrawlRankingListRedis
    {
        public override string Key(int groupId) => TemporaryKey.ToTemporaryKey(UnionBrawlKey.UnionRankingList(groupId), "forSumUp");
    }
}