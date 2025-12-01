using Data;
using System.Collections.Generic;
using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using longid = System.Int64;
using System.Linq;

namespace Script
{
    public class ServerActivePlayersRedis : WaitInitDataRedis_ServerId<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        public override string WaitKey(int serverId) => GlobalKey.ServerActivePlayersInitedFlag(serverId);

        string Key(int serverId) => GlobalKey.ServerActivePlayers(serverId);

        public async Task Add(int serverId, Dictionary<longid, int> dict)
        {
            await GetDb().SortedSetAddAsync(Key(serverId), dict.Select(kv => new SortedSetEntry(kv.Key, kv.Value)).ToArray());
        }

        public async Task<int> GetCount(int serverId)
        {
            return (int)await GetDb().SortedSetLengthAsync(Key(serverId));
        }

        public async Task Add(int serverId, longid playerId, int becomeInactiveTimeS)
        {
            await GetDb().SortedSetAddAsync(Key(serverId), playerId, becomeInactiveTimeS);
        }

        public async Task Remove(int serverId, longid playerId)
        {
            await GetDb().SortedSetRemoveAsync(Key(serverId), playerId);
        }

        public async Task Remove(int serverId, List<longid> playerIds)
        {
            await GetDb().SortedSetRemoveAsync(Key(serverId), playerIds.Select(x => new RedisValue(x.ToString())).ToArray());
        }

        public async Task<List<longid>> GetBecomeInactives(int serverId, int timeS)
        {
            RedisValue[] values = await GetDb().SortedSetRangeByScoreAsync(Key(serverId), double.NegativeInfinity, timeS, Exclude.None, Order.Ascending);
            return values.Select(x => RedisUtils.ParseLongId(x)).ToList();
        }
    }
}