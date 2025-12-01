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
    public class TerritoryProgressOpponentsRedis: ServerScript<NormalServer>
    {
        IDatabase GetDb()
        {
            return this.server.serverData.redis_db;
        }

        public async Task Add(longid playerId, TerritoryProgressOpponentDetail detail)
        {
            var simple = TerritoryProgressOpponentSimple.FromDetail(detail);

            await Task.WhenAll(
                this.GetDb().SortedSetAddAsync(TerritoryProgressOpponentsKey.Progresses(), playerId, detail.territoryProgress),
                this.GetDb().HashSetAsync(TerritoryProgressOpponentsKey.Simples(), playerId, JsonUtils.stringify(simple)),
                this.GetDb().HashSetAsync(TerritoryProgressOpponentsKey.Details(), playerId, JsonUtils.stringify(detail))
                );
        }

        // 更新联盟id
        // 联盟保卫战要求不能随机到同联盟的对手
        public async Task SetUnionId(longid playerId, longid unionId)
        {
            var detail = await this.GetDetail(playerId);
            if (detail != null)
            {
                detail.unionId = unionId;

                var simple = TerritoryProgressOpponentSimple.FromDetail(detail);

                await Task.WhenAll(this.GetDb().HashSetAsync(TerritoryProgressOpponentsKey.Simples(), playerId, JsonUtils.stringify(simple)),
                    this.GetDb().HashSetAsync(TerritoryProgressOpponentsKey.Details(), playerId, JsonUtils.stringify(detail)));
            }
        }

        public async Task RemoveMany(NormalService service, longid[] playerIds)
        {
            service.logger.InfoFormat("TerritoryProgressOpponentsRedis.RemoveMany " + JsonUtils.stringify(playerIds));

            var values = playerIds.Select(id => new RedisValue(id.ToString())).ToArray();

            await Task.WhenAll(
                this.GetDb().SortedSetRemoveAsync(TerritoryProgressOpponentsKey.Progresses(), values),
                this.GetDb().HashDeleteAsync(TerritoryProgressOpponentsKey.Simples(), values),
                this.GetDb().HashDeleteAsync(TerritoryProgressOpponentsKey.Details(), values)
            );
        }

        public async Task<TerritoryProgressOpponentDetail> GetDetail(longid playerId)
        {
            RedisValue redisValue = await this.GetDb().HashGetAsync(TerritoryProgressOpponentsKey.Details(), playerId);
            if (redisValue.IsNullOrEmpty)
            {
                return null;
            }
            return JsonUtils.parse<TerritoryProgressOpponentDetail>(redisValue);
        }

        bool OpponentExpired(TerritoryProgressOpponentSimple simple, int nowS)
        {
            if (simple.isRobot)
            {
                return false;
            }

            if (simple.territoryProgress >= 200)
            {
                return false;
            }

            // 第 1 关：1 周，之后每关加 1 天
            long expireS = 86400 * (6 + simple.territoryProgress);
            if (nowS - simple.timeS < expireS)
            {
                return false;
            }

            return true;
        }

        static bool OnlyPlayer(RedisValue[] redisValues, longid playerId)
        {
            return playerId != 0 && redisValues != null && redisValues.Length == 1 && RedisUtils.ParseLong(redisValues[0]) == playerId;
        }

        const int c_autoExpandDelta = 5;
        const int c_autoExpandCount = 3;
        async Task<(TerritoryProgressOpponentDetail, bool)> RandomOnce(
            NormalService service,
            Random random,
            longid notUnionId, longid notPlayerId,
            int territoryProgress, int lowerDelta /* 有符号 */, int upperDelta)
        {
            MyDebug.Assert(c_autoExpandDelta >= 0);
            MyDebug.Assert(c_autoExpandCount >= 0);

            int min = territoryProgress + lowerDelta;
            int max = territoryProgress + upperDelta;

            RedisValue[] playerIdValues = null;
            int expandCount = 0;
            while (true)
            {
                // 注：这里不能指定 take，否则都是离 min 比较近的
                playerIdValues = await this.GetDb().SortedSetRangeByScoreAsync(TerritoryProgressOpponentsKey.Progresses(),
                    min, max,
                    Exclude.None, Order.Ascending);

                if (playerIdValues.Length > 0 && !OnlyPlayer(playerIdValues, notPlayerId))
                {
                    break;
                }

                if (expandCount >= c_autoExpandCount)
                {
                    break;
                }

                expandCount++;

                min -= c_autoExpandDelta;
                max += c_autoExpandDelta;
            }

            if (playerIdValues.Length == 0)
            {
                RedisValue[] left = await this.GetDb().SortedSetRangeByScoreAsync(TerritoryProgressOpponentsKey.Progresses(),
                        double.NegativeInfinity, min,
                        Exclude.None, Order.Descending,
                        skip: 0, take: 10);

                RedisValue[] right = await this.GetDb().SortedSetRangeByScoreAsync(TerritoryProgressOpponentsKey.Progresses(),
                        max, double.PositiveInfinity,
                        Exclude.None, Order.Ascending,
                        skip: 0, take: 10);

                playerIdValues = left.Concat(right).ToArray();
            }

            if (playerIdValues.Length == 0)
            {
                return (null, false);
            }

            RedisValue[] simpleValues = await this.GetDb().HashGetAsync(TerritoryProgressOpponentsKey.Simples(), playerIdValues);
            var simples = new List<TerritoryProgressOpponentSimple>();

            var expireIds = new List<longid>();
            int nowS = TimeUtils.GetTimeS();

            for (int i = 0; i < simpleValues.Length; i++)
            {
                RedisValue v = simpleValues[i];

                // 由于在随机时也可能移除，所以有可能取到的 simple 是 null。此时允许 tryAgain
                if (v.IsNullOrEmpty)
                {
                    continue;
                }

                var s = JsonUtils.parse<TerritoryProgressOpponentSimple>(v);
                if (this.OpponentExpired(s, nowS))
                {
                    expireIds.Add(s.playerId);
                }

                simples.Add(s);
            }

            if (simples.Count == 0)
            {
                return (null, true); // canTryAgain
            }

            // 以下2步排除，如果 list 没东西了，就不排除了
            if (notPlayerId != 0 && simples.Count > 1)
            {
                for (int i = 0; i < simples.Count; i++)
                {
                    var s = simples[i];
                    if (s.playerId == notPlayerId)
                    {
                        simples.RemoveAt(i);
                        break;
                    }
                }
            }

            if (notUnionId != 0)
            {
                if (simples.Exists(s => s.unionId != notUnionId))
                {
                    simples = simples.Where(s => s.unionId != notUnionId).ToList();
                }
                else
                {
                    // 随到相同联盟
                }
            }

            longid playerId = simples[random.Next(0, simples.Count)].playerId;
            TerritoryProgressOpponentDetail detail = await this.GetDetail(playerId);
            if (detail == null)
            {
                // 由于在随机时也可能移除，所以有可能取到的 detail 是 null。此时允许 tryAgain
                // canTryAgain
                return (null, true);
            }

            if (expireIds.Count > 0)
            {
                this.RemoveMany(service, expireIds.ToArray()).Forget(service);
            }

            return (detail, false);
        }

        public async Task<TerritoryProgressOpponentDetail> RandomOpponent(
            NormalService service,
            Random random,
            longid notUnionId, longid notPlayerId,
            int territoryProgress, int lowerDelta /* 有符号 */, int upperDelta)
        {
            int tryCount = 0;
            while (true) // 这里的 while 是防止随到的人恰好又被移除
            {
                (TerritoryProgressOpponentDetail opponentDetail, bool canTryAgain) = await this.RandomOnce(
                    service,
                    random,
                    notUnionId, notPlayerId,
                    territoryProgress, lowerDelta /* 有符号 */, upperDelta);

                if (opponentDetail != null)
                {
                    opponentDetail.Ensure();
                    return opponentDetail;
                }

                if (!canTryAgain)
                {
                    return null;
                }

                tryCount++;
                if (tryCount >= 10)
                {
                    return null;
                }
            }
        }
    }
}