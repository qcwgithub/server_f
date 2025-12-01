// using UnityEngine;
// using System.Linq;
// using System;
// using System.Threading.Tasks;
// using Data;
// using StackExchange.Redis;
// using System.Collections.Generic;
// using longid = System.Int64;

// namespace Script
// {
//     public class WorldMapPlayerViewportRedis: IEntryScript<ScriptEntry>
//     {
//         public ScriptEntry scriptEntry { get; set; }

//         public WorldMapPlayerViewportRedis(ScriptEntry scriptEntry)
//         {
//             this.scriptEntry = scriptEntry;
//         }

//         public IDatabase GetDb()
//         {
//             return this.scriptEntry.dataEntry.redis_db;
//         }

//         public string Key_Viewport(longid unionId, int x, int y) => WorldMapKey.Viewport(unionId, x, y);
//         // public string Key_PlayerViewport(longid playerId) => WorldMapKey.PlayerViewport(playerId);

//         // 保存玩家当前的视窗
//         // public async Task SetPlayerViewports(longid playerId, List<int> viewports)
//         // {
//         //     if (viewports == null || viewports.Count == 0)
//         //     {
//         //         await this.GetDb().KeyDeleteAsync(this.Key_PlayerViewport(playerId));
//         //     }
//         //     else
//         //     {
//         //         await this.GetDb().StringSetAsync(this.Key_PlayerViewport(playerId), JsonUtils.stringify(viewports));
//         //     }
//         // }

//         // 获得当前玩家的视窗
//         // public async Task<List<int>> GetPlayerViewports(longid playerId)
//         // {
//         //     RedisValue redisValue = await this.GetDb().StringGetAsync(this.Key_PlayerViewport(playerId));
//         //     if (redisValue.IsNullOrEmpty)
//         //     {
//         //         return null;
//         //     }
//         //     return JsonUtils.parse<List<int>>(redisValue);
//         // }

//         // 热点 *****
//         public async Task AddPlayerToViewports(longid unionId, List<int> viewports, longid playerId)
//         {
//             var db = this.GetDb();

//             List<Task> tasks = new List<Task>();
//             foreach (int viewport in viewports)
//             {
//                 (int vX, int vY) = WorldMapScript.DecodeViewportKey(viewport);
//                 tasks.Add(db.SetAddAsync(this.Key_Viewport(unionId, vX, vY), playerId));
//             }

//             await Task.WhenAll(tasks);
//         }

//         // 热点 *****
//         public async Task RemovePlayerFromViewports(longid unionId, List<int> viewports, longid playerId)
//         {
//             var db = this.GetDb();

//             List<Task> tasks = new List<Task>();
//             foreach (int viewport in viewports)
//             {
//                 (int vX, int vY) = WorldMapScript.DecodeViewportKey(viewport);
//                 tasks.Add(db.SetRemoveAsync(this.Key_Viewport(unionId, vX, vY), playerId));
//             }

//             await Task.WhenAll(tasks);
//         }

//         // 获得：当前关心这个视野的玩家列表
//         public async Task<List<int>> GetViewportPlayers(longid unionId, int viewportX, int viewportY)
//         {
//             RedisValue[] redisValues = await this.GetDb().SetMembersAsync(this.Key_Viewport(unionId, viewportX, viewportY));
//             if (redisValues == null || redisValues.Length == 0)
//             {
//                 return null;
//             }
//             var playerIds = new List<int>();
//             for (int i = 0; i < redisValues.Length; i++)
//             {
//                 longid playerId = RedisUtils.ParseInt(redisValues[i]);
//                 MyDebug.Assert(playerId > 0);
//                 playerIds.Add(playerId);
//             }
//             return playerIds;
//         }

//         // 获得：当前关心这个视野的玩家列表
//         public async Task<List<int>> GetViewportsPlayers(longid unionId, HashSet<int> viewports)
//         {
//             RedisKey[] redisKeys = viewports.Select(vp =>
//             {
//                 (int vX, int vY) = WorldMapScript.DecodeViewportKey(vp);
//                 return new RedisKey(this.Key_Viewport(unionId, vX, vY));
//             }).ToArray();

//             RedisValue[] redisValues = await this.GetDb().SetCombineAsync(SetOperation.Union, redisKeys);
//             if (redisValues == null || redisValues.Length == 0)
//             {
//                 return null;
//             }

//             var playerIds = new List<int>();
//             for (int i = 0; i < redisValues.Length; i++)
//             {
//                 longid playerId = RedisUtils.ParseInt(redisValues[i]);
//                 MyDebug.Assert(playerId > 0);
//                 playerIds.Add(playerId);
//             }
//             return playerIds;
//         }
//     }
// }