using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Data;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace Script
{
    public interface ITeamRankingListRedis
    {
        Task SetNewScore(int groupId, long teamId, BigInteger newScore, int timeS, log4net.ILog logger);
        Task SetNewScoreMany(int groupId, (long, BigInteger, int)[] teamId_newScore_timeS, log4net.ILog logger);
        Task<int> GetRank(int groupId, long teamId);
        Task DeleteAll(int groupId, log4net.ILog logger);
    }

    public abstract class BaseExpeditionPartyTeamRankingListRedis : WaitInitDataRedis<NormalServer>, ITeamRankingListRedis
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        protected override string waitKey => GlobalKey.ExpeditionPartyInitedFlag();

        public abstract string Key(int groupId);

        public async Task<int> GetScore(int groupId, long teamId)
        {
            double? r = await GetDb().SortedSetScoreAsync(this.Key(groupId), teamId);
            int result = (r == null ? 0 : (int)TimeUtils.RemoveTimeFactor(r.Value));
            return result;
        }

        public async Task<(RedisKey, bool)> Clear(int groupId)
        {
            RedisKey key = this.Key(groupId);
            return (key, await this.GetDb().KeyDeleteAsync(key));
        }

        public async Task SetNewScore(int groupId, long teamId, BigInteger newScore, int timeS, log4net.ILog logger)
        {
            await GetDb().SortedSetAddAsync(this.Key(groupId), teamId, TimeUtils.AddTimeFactor(RankUtils.BigIntegerToDoubleSafe(newScore, logger), timeS));
        }

        public async Task SetNewScoreMany(int groupId, (long, BigInteger, int)[] teamId_newScore_timeS, log4net.ILog logger)
        {
            SortedSetEntry[] entries = teamId_newScore_timeS.Select(x => new SortedSetEntry(x.Item1, TimeUtils.AddTimeFactor(RankUtils.BigIntegerToDoubleSafe(x.Item2, logger), x.Item3))).ToArray();
            await GetDb().SortedSetAddAsync(this.Key(groupId), entries);
        }

        public async Task<long> GetLength(int groupId)
        {
            return await GetDb().SortedSetLengthAsync(this.Key(groupId));
        }

        public async Task<int> GetRank(int groupId, long teamId)
        {
            long? r = await GetDb().SortedSetRankAsync(this.Key(groupId), teamId, Order.Descending);
            if (r == null)
            {
                return RankUtils.INVALID_RANK;
            }
            else
            {
                return RankUtils.FromIndex((int)r.Value);
            }
        }

        public async Task GetAll(int groupId, /*OUT*/ List<long> teamIds)
        {
            RedisValue[] redisValues = await GetDb().SortedSetRangeByScoreAsync(this.Key(groupId), double.NegativeInfinity, double.PositiveInfinity, Exclude.None, Order.Descending);
            if (redisValues != null)
            {
                foreach (RedisValue v in redisValues)
                {
                    teamIds.Add(RedisUtils.ParseLongId(v));
                }
            }
        }

        public async Task GetByIndexRange(int groupId, int startIndex, int count, /*OUT*/List<long> teamIds)
        {
            int endIndex = count == -1 ? -1 : startIndex + count - 1;

            RedisValue[] redisValues = await GetDb().SortedSetRangeByRankAsync(this.Key(groupId), startIndex, endIndex, Order.Descending);
            if (redisValues != null)
            {
                foreach (RedisValue v in redisValues)
                {
                    teamIds.Add(RedisUtils.ParseLongId(v));
                }
            }
        }

        public async Task DeleteAll(int groupId, log4net.ILog logger)
        {
            logger.InfoFormat("delete key '{0}'", this.Key(groupId));
            await this.GetDb().KeyDeleteAsync(this.Key(groupId));
        }
    }

    public class ExpeditionPartyTeamRankingListRedis : BaseExpeditionPartyTeamRankingListRedis
    {
        public override string Key(int groupId) => ExpeditionPartyKey.TeamRankingList(groupId);
    }

    public class TempExpeditionPartyTeamRankingListRedis : BaseExpeditionPartyTeamRankingListRedis
    {
        public override string Key(int groupId) => TemporaryKey.ToTemporaryKey(ExpeditionPartyKey.TeamRankingList(groupId), "forSumUp");
    }
}