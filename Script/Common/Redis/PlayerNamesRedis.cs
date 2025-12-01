using Data;
using System.Collections.Generic;
using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using longid = System.Int64;

namespace Script
{
    public class PlayerNamesRedis : WaitInitDataRedis_ServerId<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }
        public override string WaitKey(int serverId) => GlobalKey.PlayerNamesInitedFlag(serverId);

        string Key(int serverId, string name) => NameKey.PlayerName(serverId, SCUtils.Base64Encode(name));

        public async Task AddManyNames(int serverId, Dictionary<longid, string> nameDict)
        {
            var array = new KeyValuePair<RedisKey, RedisValue>[nameDict.Count];
            int i = 0;

            foreach (var kv in nameDict)
            {
                array[i++] = new KeyValuePair<RedisKey, RedisValue>(
                    Key(serverId, kv.Value),
                    kv.Key
                );
            }

            await GetDb().StringSetAsync(array);
        }

        public async Task AddPlayerName(int serverId, string name, longid playerId)
        {
            await GetDb().StringSetAsync(Key(serverId, name), playerId);
        }

        public async Task DeletePlayerName(int serverId, string name)
        {
            await GetDb().KeyDeleteAsync(Key(serverId, name));
        }

        public async Task<longid> FindPlayerIdByName(int serverId, string name)
        {
            RedisValue redisValue = await GetDb().StringGetAsync(Key(serverId, name));
            return RedisUtils.ParseLongId(redisValue);
        }

        bool CanUse(RedisValue redisValue, longid allowPlayerId)
        {
            longid u = RedisUtils.ParseLongId(redisValue);
            return (u == 0 || u == allowPlayerId);
        }

        public async Task<ECode> CheckCanUseName(int serverId, string name, longid allowPlayerId)
        {
            RedisValue redisValue = await GetDb().StringGetAsync(Key(serverId, name));
            if (!this.CanUse(redisValue, allowPlayerId))
            {
                return ECode.PlayerNameExisted;
            }

            return ECode.Success;
        }
    }

}