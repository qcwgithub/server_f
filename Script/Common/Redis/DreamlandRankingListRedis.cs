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
    public abstract class BaseDreamlandRankingListRedis : WaitInitDataRedis_ServerId<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        public override string WaitKey(int serverId) => GlobalKey.DreamlandInitedFlag(serverId);

        public abstract string Key(int serverId);

        public async Task<int> GetScore(int serverId, longid playerId)
        {
            double? r = await GetDb().SortedSetScoreAsync(this.Key(serverId), playerId);
            int result = (r == null ? 0 : (int)TimeUtils.RemoveTimeFactor(r.Value));
            return result;
        }

        public async Task SetNewScore(int serverId, longid playerId, BigInteger newScore, int timeS, log4net.ILog logger)
        {
            await GetDb().SortedSetAddAsync(this.Key(serverId), playerId, TimeUtils.AddTimeFactor(RankUtils.BigIntegerToDoubleSafe(newScore, logger), timeS));
        }

        public async Task SetNewScoreMany(int serverId, (longid, BigInteger, int)[] playerId_newScore_timeS, log4net.ILog logger)
        {
            SortedSetEntry[] entries = playerId_newScore_timeS.Select(x => new SortedSetEntry(x.Item1, TimeUtils.AddTimeFactor(RankUtils.BigIntegerToDoubleSafe(x.Item2, logger), x.Item3))).ToArray();
            await GetDb().SortedSetAddAsync(this.Key(serverId), entries);
        }

        public async Task<long> GetLength(int serverId)
        {
            return await GetDb().SortedSetLengthAsync(this.Key(serverId));
        }

        public async Task<int> GetRank(int serverId, longid playerId)
        {
            long? r = await GetDb().SortedSetRankAsync(this.Key(serverId), playerId, Order.Descending);
            if (r == null)
            {
                return RankUtils.INVALID_RANK;
            }
            else
            {
                return RankUtils.FromIndex((int)r.Value);
            }
        }

        public async Task GetAll(int serverId, /*OUT*/ List<longid> playerIds)
        {
            RedisValue[] redisValues = await GetDb().SortedSetRangeByScoreAsync(this.Key(serverId), double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Descending);
            if (redisValues != null)
            {
                foreach (RedisValue v in redisValues)
                {
                    playerIds.Add(RedisUtils.ParseLongId(v));
                }
            }
        }

        public async Task GetByIndexRange(int serverId, int startIndex, int count, /*OUT*/List<longid> playerIds)
        {
            int endIndex = count == -1 ? -1 : startIndex + count - 1;

            RedisValue[] redisValues = await GetDb().SortedSetRangeByRankAsync(this.Key(serverId), startIndex, endIndex, Order.Descending);
            if (redisValues != null)
            {
                foreach (RedisValue v in redisValues)
                {
                    playerIds.Add(RedisUtils.ParseLongId(v));
                }
            }
        }

        public async Task DeleteAll(int serverId, log4net.ILog logger)
        {
            logger.InfoFormat("delete key '{0}'", this.Key(serverId));
            await this.GetDb().KeyDeleteAsync(this.Key(serverId));
        }
    }

    public class DreamlandRankingListRedis : BaseDreamlandRankingListRedis
    {
        public override string Key(int serverId) => DreamlandKey.RankingList(serverId);
    }

    public class TempDreamlandRankingListRedis : BaseDreamlandRankingListRedis
    {
        public override string Key(int serverId) => TemporaryKey.ToTemporaryKey(DreamlandKey.RankingList(serverId), "forSumUp");
    }
}