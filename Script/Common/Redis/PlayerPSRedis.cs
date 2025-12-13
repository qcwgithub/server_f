using System.Diagnostics;
using System;
using System.Linq;
using Data;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Script
{
    /*
    清空Redis能否正常 | YES，是运行时数据
    */
    public class PlayerPSRedis: ServerScript
    {
        public PlayerPSRedis(Server server) : base(server)
        {
            
        }

        public IDatabase GetDb()
        {
            return this.server.data.redis_db;
        }

        public string Key(long playerId) => UserKey.PSId(playerId);

        public async Task SetPSId(long playerId, int playerServiceId, int secondsToLive)
        {
            await GetDb().StringSetAsync(UserKey.PSId(playerId), playerServiceId, TimeSpan.FromSeconds(secondsToLive));
        }

        public async Task<int> GetPSId(long playerId)
        {
            RedisValue redisValue = await GetDb().StringGetAsync(UserKey.PSId(playerId));
            return RedisUtils.ParseInt(redisValue);
        }

        public async Task<List<int>> GetMany(List<long> playerIds)
        {
            RedisValue[] redisValues = await GetDb().StringGetAsync(playerIds.Select(_ => new RedisKey(UserKey.PSId(_))).ToArray());
            return redisValues.Select(_ => RedisUtils.ParseInt(_)).ToList();
        }

        public async void DeletePSId(long playerId)
        {
            await GetDb().KeyDeleteAsync(UserKey.PSId(playerId));
        }
    }
}