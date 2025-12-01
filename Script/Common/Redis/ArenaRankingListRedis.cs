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
    public abstract class ArenaRankingListRedis : WaitInitDataRedis<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        protected override string waitKey => GlobalKey.ArenaInitedFlag();

        public abstract string Key(int groupType, int groupId);

        public async Task<int> GetScore(int groupType, int groupId, longid playerId)
        {
            double? r = await GetDb().SortedSetScoreAsync(this.Key(groupType, groupId), playerId);
            int result = (r == null ? 0 : (int)TimeUtils.RemoveTimeFactor(r.Value));
            return result;
        }

        public virtual async Task SetNewScore(int groupType, int groupId, longid playerId, int newScore, int timeS)
        {
            await GetDb().SortedSetAddAsync(this.Key(groupType, groupId), playerId, TimeUtils.AddTimeFactor(newScore, timeS));
        }

        public async Task<int> GetLength(int groupType, int groupId)
        {
            return (int)await GetDb().SortedSetLengthAsync(this.Key(groupType, groupId));
        }

        public async Task<int> GetRank(int groupType, int groupId, longid playerId)
        {
            long? r = await GetDb().SortedSetRankAsync(this.Key(groupType, groupId), playerId, Order.Descending);
            if (r == null)
            {
                return RankUtils.INVALID_RANK;
            }
            else
            {
                return RankUtils.FromIndex((int)r.Value);
            }
        }

        public async Task GetAll(int groupType, int groupId, /*OUT*/ List<longid> playerIds)
        {
            RedisValue[] redisValues = await GetDb().SortedSetRangeByScoreAsync(this.Key(groupType, groupId), double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Descending);
            if (redisValues != null)
            {
                foreach (RedisValue v in redisValues)
                {
                    playerIds.Add(RedisUtils.ParseLongId(v));
                }
            }
        }

        public async Task GetByIndexRange(int groupType, int groupId, int startIndex, int count, /*OUT*/List<longid> playerIds)
        {
            int endIndex = startIndex + count - 1;

            RedisValue[] redisValues = await GetDb().SortedSetRangeByRankAsync(this.Key(groupType, groupId), startIndex, endIndex, Order.Descending);
            if (redisValues != null)
            {
                foreach (RedisValue v in redisValues)
                {
                    playerIds.Add(RedisUtils.ParseLongId(v));
                }
            }
        }

        // takeNum 可以是 -1
        public async Task GetByScoreRangeTakeLimit(int groupType, int groupId, double min, double max, bool ascending, int takeNum, /*OUT*/List<longid> playerIds)
        {
            MyDebug.Assert(min >= 0);
            MyDebug.Assert(max >= min);

            min = TimeUtils.AddTimeFactorMin(min);
            max = TimeUtils.AddTimeFactorMax(max);

            RedisValue[] redisValues = await GetDb().SortedSetRangeByScoreAsync(this.Key(groupType, groupId), min, max, Exclude.None, ascending ? Order.Ascending : Order.Descending, 0, takeNum);
            if (redisValues != null)
            {
                foreach (RedisValue v in redisValues)
                {
                    playerIds.Add(RedisUtils.ParseLongId(v));
                }
            }
        }

        public async Task DeleteAll(int groupType, int groupId)
        {
            await this.GetDb().KeyDeleteAsync(this.Key(groupType, groupId));
        }
    }
}