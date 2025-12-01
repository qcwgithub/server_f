using UnityEngine;
using System.Linq;
using System;
using System.Threading.Tasks;
using Data;
using StackExchange.Redis;
using System.Collections.Generic;
using longid = System.Int64;

namespace Script
{
    public class WorldMapPlayerIndex2PlayerIdRedis : WaitInitDataRedis_ServerId<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }
        public override string WaitKey(int serverId) => GlobalKey.WorldMapPlayerInitedFlag(serverId);

        public string Key(string mapId, int playerIndex) => WorldMapKey.PlayerIndex2PlayerId(mapId, playerIndex);

        public async Task<longid> Get(string mapId, int playerIndex)
        {
            MyDebug.Assert(mapId != null);

            RedisValue redisValue = await this.GetDb().StringGetAsync(this.Key(mapId, playerIndex));
            return RedisUtils.ParseLongId(redisValue);
        }

        public async Task<bool> Exist(string mapId, int playerIndex)
        {
            MyDebug.Assert(mapId != null);

            return await this.GetDb().KeyExistsAsync(this.Key(mapId, playerIndex));
        }

        // 返回值：playerIndex -> playerId
        public async Task<Dictionary<int, longid>> GetMany(string mapId, List<int> playerIndexes)
        {
            MyDebug.Assert(mapId != null);

            var redisKeys = playerIndexes.Select(c => new RedisKey(this.Key(mapId, c))).ToArray();

            RedisValue[] redisValues = await this.GetDb().StringGetAsync(redisKeys);

            var dict = new Dictionary<int, longid>();
            for (int i = 0; i < redisValues.Length; i++)
            {
                longid playerId = RedisUtils.ParseLongId(redisValues[i]);
                if (playerId != 0) // 排除掉 Value 是 0 的
                {
                    dict[playerIndexes[i]] = playerId;
                }
            }

            return dict;
        }

        public async Task<List<longid>> GetMany(string mapId, List<int> playerIndexes, bool fill0IfNotExist)
        {
            MyDebug.Assert(mapId != null);

            var redisKeys = playerIndexes.Select(index => new RedisKey(this.Key(mapId, index))).ToArray();

            RedisValue[] redisValues = await this.GetDb().StringGetAsync(redisKeys);

            var list = new List<longid>();
            for (int i = 0; i < redisValues.Length; i++)
            {
                longid playerId = RedisUtils.ParseLongId(redisValues[i]);
                if (fill0IfNotExist || playerId != 0)
                {
                    list.Add(playerId);
                }
            }

            return list;
        }

        public Task Remove(string mapId, int playerIndex)
        {
            MyDebug.Assert(mapId != null);

            return this.GetDb().KeyDeleteAsync(new RedisKey(this.Key(mapId, playerIndex)));
        }

        public Task AddMany(string mapId, Dictionary<int, longid> playerIndex2PlayerId)
        {
            MyDebug.Assert(mapId != null);

            return this.GetDb().StringSetAsync(
                playerIndex2PlayerId
                .Select(kv => new KeyValuePair<RedisKey, RedisValue>(new RedisKey(this.Key(mapId, kv.Key)), new RedisValue(kv.Value.ToString())))
                .ToArray());
        }

        public Task Add(string mapId, int playerIndex, longid playerId)
        {
            MyDebug.Assert(mapId != null);

            return this.GetDb().StringSetAsync(new RedisKey(this.Key(mapId, playerIndex)), playerId);
        }
    }
}