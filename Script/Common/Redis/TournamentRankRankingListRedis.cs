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
    public abstract class BaseTournamentRankRankingListRedis : GWaitInitDataRedis<GroupServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        protected override string waitKey => GGlobalKey.TournamentRankInitedFlag();

        public abstract string Key();

        public async Task<int> GetScore(longid playerId)
        {
            double? r = await GetDb().SortedSetScoreAsync(this.Key(), playerId);
            int result = (r == null ? 0 : (int)TimeUtils.RemoveTimeFactor(r.Value));
            return result;
        }

        public async Task SetNewScore(longid playerId, int newScore, int timeS)
        {
            await GetDb().SortedSetAddAsync(this.Key(), playerId, TimeUtils.AddTimeFactor(newScore, timeS));
        }

        public async Task SetNewScoreMany((longid, int, int)[] playerId_newScore_timeS)
        {
            SortedSetEntry[] entries = playerId_newScore_timeS.Select(x => new SortedSetEntry(x.Item1, TimeUtils.AddTimeFactor(x.Item2, x.Item3))).ToArray();
            await GetDb().SortedSetAddAsync(this.Key(), entries);
        }

        public async Task<long> GetLength()
        {
            return await GetDb().SortedSetLengthAsync(this.Key());
        }

        public async Task<int> GetRank(longid playerId)
        {
            long? r = await GetDb().SortedSetRankAsync(this.Key(), playerId, Order.Descending);
            if (r == null)
            {
                return RankUtils.INVALID_RANK;
            }
            else
            {
                return RankUtils.FromIndex((int)r.Value);
            }
        }

        public async Task GetAll(/*OUT*/ List<longid> playerIds)
        {
            RedisValue[] redisValues = await GetDb().SortedSetRangeByScoreAsync(this.Key(), double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Descending);
            if (redisValues != null)
            {
                foreach (RedisValue v in redisValues)
                {
                    playerIds.Add(RedisUtils.ParseLongId(v));
                }
            }
        }

        public async Task GetByIndexRange(int startIndex, int count, /*OUT*/List<longid> playerIds)
        {
            int endIndex = startIndex + count - 1;

            RedisValue[] redisValues = await GetDb().SortedSetRangeByRankAsync(this.Key(), startIndex, endIndex, Order.Descending);
            if (redisValues != null)
            {
                foreach (RedisValue v in redisValues)
                {
                    playerIds.Add(RedisUtils.ParseLongId(v));
                }
            }
        }

        public async Task DeleteAll(log4net.ILog logger)
        {
            logger.InfoFormat("delete key '{0}'", this.Key());
            await this.GetDb().KeyDeleteAsync(this.Key());
        }
    }

    public class TournamentRankRankingListRedis : BaseTournamentRankRankingListRedis
    {
        public override string Key() => TournamentRankKey.RankingList();
    }

    public class TempTournamentRankRankingListRedis : BaseTournamentRankRankingListRedis
    {
        public override string Key() => TemporaryKey.ToTemporaryKey(TournamentRankKey.RankingList(), "forSumUp");
    }
}