using System.Diagnostics;
using System;
using System.Linq;
using Data;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;
using longid = System.Int64;

namespace Script
{
    /*
    清空Redis能否正常 | YES，是运行时数据
    */
    public class PlayerPSRedis: ServerScript<NormalServer>
    {
        public IDatabase GetDb()
        {
            return this.server.serverData.redis_db;
        }

        public string Key(longid playerId) => PlayerKey.PSId(playerId);

        public async Task SetPSId(longid playerId, int playerServiceId, int secondsToLive)
        {
            await GetDb().StringSetAsync(PlayerKey.PSId(playerId), playerServiceId, TimeSpan.FromSeconds(secondsToLive));
        }

        public async Task<int> GetPSId(longid playerId)
        {
            RedisValue redisValue = await GetDb().StringGetAsync(PlayerKey.PSId(playerId));
            return RedisUtils.ParseInt(redisValue);
        }

        public async Task<List<int>> GetMany(List<longid> playerIds)
        {
            RedisValue[] redisValues = await GetDb().StringGetAsync(playerIds.Select(_ => new RedisKey(PlayerKey.PSId(_))).ToArray());
            return redisValues.Select(_ => RedisUtils.ParseInt(_)).ToList();
        }

        public async void DeletePSId(longid playerId)
        {
            await GetDb().KeyDeleteAsync(PlayerKey.PSId(playerId));
        }
    }
}