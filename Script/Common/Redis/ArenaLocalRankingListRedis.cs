using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using StackExchange.Redis;
using longid = System.Int64;

namespace Script
{
    public abstract class ArenaLocalRankingListRedis_Base : WaitInitDataRedis<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        protected override string waitKey => GlobalKey.ArenaInitedFlag();

        public abstract string Key(int serverId);

        public async Task<int> GetScore(int serverId, longid playerId)
        {
            double? r = await GetDb().SortedSetScoreAsync(this.Key(serverId), playerId);
            int result = (r == null ? 0 : (int)TimeUtils.RemoveTimeFactor(r.Value));
            return result;
        }

        public virtual async Task SetNewScore(int serverId, longid playerId, int newScore, int timeS)
        {
            await GetDb().SortedSetAddAsync(this.Key(serverId), playerId, TimeUtils.AddTimeFactor(newScore, timeS));
        }

        public async Task<int> GetLength(int serverId)
        {
            return (int)await GetDb().SortedSetLengthAsync(this.Key(serverId));
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
            int endIndex = startIndex + count - 1;

            RedisValue[] redisValues = await GetDb().SortedSetRangeByRankAsync(this.Key(serverId), startIndex, endIndex, Order.Descending);
            if (redisValues != null)
            {
                foreach (RedisValue v in redisValues)
                {
                    playerIds.Add(RedisUtils.ParseLongId(v));
                }
            }
        }

        // takeNum 可以是 -1
        public async Task GetByScoreRangeTakeLimit(int serverId, double min, double max, bool ascending, int takeNum, /*OUT*/List<longid> playerIds)
        {
            MyDebug.Assert(min >= 0);
            MyDebug.Assert(max >= min);

            min = TimeUtils.AddTimeFactorMin(min);
            max = TimeUtils.AddTimeFactorMax(max);

            RedisValue[] redisValues = await GetDb().SortedSetRangeByScoreAsync(this.Key(serverId), min, max, Exclude.None, ascending ? Order.Ascending : Order.Descending, 0, takeNum);
            if (redisValues != null)
            {
                foreach (RedisValue v in redisValues)
                {
                    playerIds.Add(RedisUtils.ParseLongId(v));
                }
            }
        }

        public async Task DeleteAll(int serverId)
        {
            await this.GetDb().KeyDeleteAsync(this.Key(serverId));
        }
    }

    public class ArenaLocalRankingListRedis : ArenaLocalRankingListRedis_Base
    {
        public override string Key(int serverId)
        {
            string key = ArenaKey.LocalRankingList(serverId);
            return key;
        }

        public async Task<int> CalucatePageCount(int serverId, int? length, int PER_PAGE)
        {
            if (length == null)
            {
                length = await this.GetLength(serverId);
            }
            int pageCount = length.Value / PER_PAGE;
            if (length % PER_PAGE != 0)
            {
                pageCount += 1;
            }
            return pageCount;
        }
    }

    // public class TempArenaLocalRankingListRedis : ArenaLocalRankingListRedis_Base
    // {
    //     public override string Key(int serverId)
    //     {
    //         string key = TemporaryKey.ToTemporaryKey(ArenaKey.LocalRankingList(serverId), "forSumUp");
    //         return key;
    //     }
    // }
}