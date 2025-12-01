using Data;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace Script
{
    public class WorldMapSearchDataRedisEx : WaitInitDataRedis_ServerId<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }
        public override string WaitKey(int serverId) => GlobalKey.WorldMapResourceInitedFlag(serverId);

        public string Key(string mapId, int level) => WorldMapKey.MapSearchDataEx(mapId, level);

        public async Task AddMany(string mapId, int level, Dictionary<string, stResourceSearchShort> dict)
        {
            await this.GetDb().HashSetAsync(this.Key(mapId, level), dict.Select(x => new HashEntry(x.Key, JsonUtils.stringify(x.Value))).ToArray());
        }

        public async Task<bool> Add(string mapId, int level, string resourceId, stResourceSearchShort short_)
        {
            bool newlyAdded = await this.GetDb().HashSetAsync(this.Key(mapId, level), resourceId, JsonUtils.stringify(short_));
            return newlyAdded;
        }

        public async Task<bool> Remove(string mapId, int level, string resourceId)
        {
            bool removed = await this.GetDb().HashDeleteAsync(this.Key(mapId, level), resourceId);
            return removed;
        }

        // public async Task<Dictionary<string, stResourceSearchShort>> GetAll(string mapId, int level)
        // {
        //     HashEntry[] entries = await this.GetDb().HashGetAllAsync(this.Key(mapId, level));
        //     var dict = new Dictionary<string, stResourceSearchShort>();
        //     foreach (HashEntry entry in entries)
        //     {
        //         dict[entry.Name] = JsonUtils.parse<stResourceSearchShort>(entry.Value);
        //     }
        //     return dict;
        // }

        // My Public
        // My Union
        // Other Maps
        public async Task<List<stResourceSearchShortEx>> GetAllEx(string mapId, int level)
        {
            HashEntry[] entries = await this.GetDb().HashGetAllAsync(this.Key(mapId, level));
            var dict = new Dictionary<string, stResourceSearchShort>();
            foreach (HashEntry entry in entries)
            {
                dict[entry.Name] = JsonUtils.parse<stResourceSearchShort>(entry.Value);
            }

            return dict.Select(kv =>
                {
                    WorldMapResourceInfo.DecodeResourceId(kv.Key, out int resourceIndex, out int localIndex);
                    return new stResourceSearchShortEx { resourceId = kv.Key, resourceIndex = resourceIndex, collectingPlayerId = kv.Value.p, spTimeS = kv.Value.t };
                })
                .ToList();
        }
    }
}