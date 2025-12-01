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
    public class ExpeditionPartyPlayerOpponentRedis : WaitInitDataRedis<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        protected override string waitKey => GlobalKey.ExpeditionPartyInitedFlag();

        string Key(int groupId) => ExpeditionPartyKey.PlayerOpponent(groupId);

        public async Task<(RedisKey, bool)> Clear(int groupId)
        {
            RedisKey key = this.Key(groupId);
            return (key, await GetDb().KeyDeleteAsync(key));
        }

        // weight 万分比
        public async Task Add(int groupId, longid playerId, int weight)
        {
            await GetDb().HashSetAsync(Key(groupId), playerId, weight);
        }

        public async Task Adds(int groupId, Dictionary<longid, int> playerId_weights)
        {
            await GetDb().HashSetAsync(Key(groupId), playerId_weights.Select(kv => new HashEntry(kv.Key, kv.Value)).ToArray());
        }

        public async Task Remove(int groupId, longid playerId)
        {
            await GetDb().HashDeleteAsync(Key(groupId), playerId);
        }

        public async Task<List<(longid, int)>> GetAll(int groupId)
        {
            HashEntry[] entries = await GetDb().HashGetAllAsync(Key(groupId));
            return entries.Select(entry => (RedisUtils.ParseLongId(entry.Name), RedisUtils.ParseInt(entry.Value))).ToList();
        }

        public async Task DeleteAll(int groupId, log4net.ILog logger)
        {
            logger.InfoFormat("delete key '{0}'", this.Key(groupId));
            await GetDb().KeyDeleteAsync(Key(groupId));
        }
    }
}