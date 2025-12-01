using Data;
using System.Collections.Generic;
using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Linq;
using longid = System.Int64;

namespace Script
{
    // 所有活着的联盟
    public class AllUnionIdsRedis : WaitInitDataRedis_ServerId<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        public string Key(stParentServerId parentServerId) => GlobalKey.AllUnionIds(parentServerId);
        public override string WaitKey(int serverId) => GlobalKey.AllUnionIdsInitedFlag(serverId);
        public async Task<(RedisKey, bool)> Clear(stParentServerId parentServerId)
        {
            RedisKey key = this.Key(parentServerId);
            return (key, await this.GetDb().KeyDeleteAsync(key));
        }
        public async Task AddUnionIds(stParentServerId parentServerId, List<longid> idList)
        {
            var array = idList.Select(id => new RedisValue(id.ToString())).ToArray();
            await GetDb().SetAddAsync(Key(parentServerId), array);
        }

        public async Task AddUnionId(stParentServerId parentServerId, longid unionId)
        {
            await GetDb().SetAddAsync(Key(parentServerId), unionId.ToString());
        }

        public async Task<bool> Contains(stParentServerId parentServerId, longid unionId)
        {
            return await GetDb().SetContainsAsync(Key(parentServerId), unionId);
        }

        public async Task DelUnionId(stParentServerId parentServerId, longid unionId)
        {
            await GetDb().SetRemoveAsync(Key(parentServerId), unionId);
        }

        public async Task<List<longid>> GetAll(stParentServerId parentServerId)
        {
            RedisValue[] redisValues = await GetDb().SetMembersAsync(Key(parentServerId));
            var retList = redisValues.Select(v => RedisUtils.ParseLongId(v)).ToList();
            return retList;
        }

        public async Task<List<longid>> RandomGetUnionIds(stParentServerId parentServerId, int count)
        {
            RedisValue[] redisValues = await GetDb().SetRandomMembersAsync(Key(parentServerId), count);
            if (redisValues == null || redisValues.Length == 0)
            {
                return new List<longid>();
            }
            var retList = redisValues.Select(v => RedisUtils.ParseLongId(v)).ToList();
            return retList;
        }
    }
}