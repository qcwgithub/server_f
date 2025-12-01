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
    public abstract class ChampionRankingListRedis : WaitInitDataRedis<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        protected override string waitKey => GlobalKey.ChampionInitedFlag();

        public abstract string Key(int groupType, int groupId);

        public virtual async Task SetNewScore(int groupType, int groupId, longid playerId, long newScore)
        {
            await GetDb().SortedSetAddAsync(this.Key(groupType, groupId), playerId, newScore);
        }

        public virtual async Task SetNewScoreMany(int groupType, int groupId, Dictionary<longid, long> playerId2Scores, bool overrideWhole)
        {
            if (overrideWhole)
            {
                await GetDb().KeyDeleteAsync(this.Key(groupType, groupId));
            }

            var entries = playerId2Scores.Select(kv => new SortedSetEntry(kv.Key, kv.Value)).ToArray();
            await GetDb().SortedSetAddAsync(this.Key(groupType, groupId), entries);
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

        public async Task<long> GetScore(int groupType, int groupId, longid playerId)
        {
            double? d = await GetDb().SortedSetScoreAsync(this.Key(groupType, groupId), playerId);
            return d == null ? 0 : (long)d.Value;
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

        // 战斗时，根据 rank 获取 playerId2，看看有没有变化
        public async Task<longid> GetPlayerIdByRank(int groupType, int groupId, int rank)
        {
            MyDebug.Assert(rank != RankUtils.INVALID_RANK);
            RedisValue[] redisValues = await GetDb().SortedSetRangeByRankAsync(this.Key(groupType, groupId), rank - 1, rank - 1, Order.Descending);
            if (redisValues == null || redisValues.Length != 1)
            {
                return 0;
            }
            return RedisUtils.ParseLongId(redisValues[0]);
        }

        public async Task GetByRankRange(int groupType, int groupId, int startRank, int endRank, /*OUT*/List<longid> playerIds)
        {
            RedisValue[] redisValues = await GetDb().SortedSetRangeByRankAsync(this.Key(groupType, groupId), startRank - 1, endRank - 1, Order.Descending);
            foreach (RedisValue v in redisValues)
            {
                playerIds.Add(RedisUtils.ParseLongId(v));
            }
        }

        public async Task DeleteAll(int groupType, int groupId)
        {
            await this.GetDb().KeyDeleteAsync(this.Key(groupType, groupId));
        }
    }
}