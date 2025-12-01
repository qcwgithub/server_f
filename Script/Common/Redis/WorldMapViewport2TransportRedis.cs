using System.Diagnostics;
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
    // viewport -> (playerId1, playerId2)
    public class WorldMapViewport2TransportRedis : WaitInitDataRedis_ServerId<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }
        public override string WaitKey(int serverId) => GlobalKey.WorldMapPlayerInitedFlag(serverId);

        public string Key(string mapId, int x, int y) => WorldMapKey.ViewportTransport(mapId, x, y);

        public async Task AddTransportToViewports(string mapId, HashSet<int> viewports, longid playerId)
        {
            if (playerId == 0)
            {
                MyDebug.Assert(false);
                return;
            }

            var db = this.GetDb();

            List<Task> tasks = new List<Task>();
            foreach (int viewport in viewports)
            {
                (int vX, int vY) = WorldMapScript.DecodeViewportKey(viewport);
                tasks.Add(db.SetAddAsync(this.Key(mapId, vX, vY), playerId));
            }

            await Task.WhenAll(tasks);
        }

        public async Task RemoveTransportFromViewports(string mapId, HashSet<int> viewports, longid playerId)
        {
            if (playerId == 0)
            {
                MyDebug.Assert(false);
                return;
            }

            var db = this.GetDb();

            List<Task> tasks = new List<Task>();
            foreach (int viewport in viewports)
            {
                (int vX, int vY) = WorldMapScript.DecodeViewportKey(viewport);
                tasks.Add(db.SetRemoveAsync(this.Key(mapId, vX, vY), playerId));
            }

            await Task.WhenAll(tasks);
        }

        public async Task<List<longid>> GetTransportsInViewports(string mapId, List<int> viewports, bool fill0IfNotExist)
        {
            var db = this.GetDb();

            var redisKeys = new RedisKey[viewports.Count];
            int index = 0;
            foreach (int viewport in viewports)
            {
                (int vX, int vY) = WorldMapScript.DecodeViewportKey(viewport);
                redisKeys[index++] = this.Key(mapId, vX, vY);
            }

            RedisValue[] redisValues = await db.SetCombineAsync(SetOperation.Union, redisKeys);
            if (redisValues == null || redisValues.Length == 0)
            {
                return null;
            }

            var playerIds = new List<longid>();
            foreach (RedisValue redisValue in redisValues)
            {
                longid playerId = RedisUtils.ParseLongId(redisValue);
                MyDebug.Assert(playerId != 0);
                if (fill0IfNotExist || playerId != 0)
                {
                    playerIds.Add(playerId);
                }
            }
            return playerIds;
        }

        public async Task<List<longid>> GetTransportsInViewport(string mapId, int viewport, bool fill0IfNotExist)
        {
            var db = this.GetDb();

            (int vX, int vY) = WorldMapScript.DecodeViewportKey(viewport);
            RedisKey redisKey = this.Key(mapId, vX, vY);

            RedisValue[] redisValues = await db.SetMembersAsync(redisKey);
            if (redisValues == null || redisValues.Length == 0)
            {
                return null;
            }

            var playerIds = new List<longid>();
            foreach (RedisValue redisValue in redisValues)
            {
                longid playerId = RedisUtils.ParseLongId(redisValue);
                MyDebug.Assert(playerId != 0);
                if (fill0IfNotExist || playerId != 0)
                {
                    playerIds.Add(playerId);
                }
            }
            return playerIds;
        }
    }
}