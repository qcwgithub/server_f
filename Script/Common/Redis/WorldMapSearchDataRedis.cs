using System.Threading.Tasks;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace Script
{
    public class WorldMapSearchDataRedis : WaitInitDataRedis_ServerId<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }
        public override string WaitKey(int serverId) => GlobalKey.WorldMapResourceInitedFlag(serverId);

        public string Key(string mapId, int level) => WorldMapKey.MapSearchData(mapId, level);

        public async Task AddMany(string mapId, int level, List<string> resourceIds)
        {
            await this.GetDb().SetAddAsync(this.Key(mapId, level), resourceIds.Select(x => new RedisValue(x)).ToArray());
        }

        public async Task Add(string mapId, int level, string resourceId)
        {
            await this.GetDb().SetAddAsync(this.Key(mapId, level), resourceId);
        }

        public async Task Remove(string mapId, int level, string resourceId)
        {
            await this.GetDb().SetRemoveAsync(this.Key(mapId, level), resourceId);
        }

        public async Task<List<string>> GetAll(string mapId, int level)
        {
            RedisValue[] redisValues = await this.GetDb().SetMembersAsync(this.Key(mapId, level));
            return redisValues.Select(v => v.ToString()).ToList();
        }

        public async Task<List<string>> Random(string mapId, int level, int count)
        {
            RedisValue[] redisValues = await this.GetDb().SetRandomMembersAsync(this.Key(mapId, level), count);
            return redisValues.Select(v => v.ToString()).ToList();
        }
    }
}