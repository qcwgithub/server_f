using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Data;
using StackExchange.Redis;
using System.Collections.Generic;
using longid = System.Int64;

namespace Script
{
    public class UnionBrawlRedis : WaitInitDataRedis<GroupServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }
        protected override string waitKey => GGlobalKey.UnionBrawlInitedFlag();

        //// playerId 对应的 unionId，开战时初始化，之后不变

        public async Task<longid> PlayerToUnionId_Get(longid playerId)
        {
            RedisValue redisValue = await GetDb().HashGetAsync(UnionBrawlKey.PlayerToUnionId(), playerId);
            return RedisUtils.ParseLongId(redisValue);
        }

        // 开战初始化 ✓
        // GGlobal 初始化 ✓
        public async Task PlayerToUnionId_Add(List<longid> playerIds, longid unionId)
        {
            await GetDb().HashSetAsync(UnionBrawlKey.PlayerToUnionId(), playerIds.Select(x => new HashEntry(x, unionId)).ToArray());
        }

        public async Task PlayerToUnionId_Clear()
        {
            await GetDb().KeyDeleteAsync(UnionBrawlKey.PlayerToUnionId());
        }

        //// Signup Queue 报名队列

        // 正常初始化 ✓
        // GGlobal 初始化 ✓
        public async Task SignupQueue_Add(longid unionId)
        {
            MyDebug.Assert(unionId > 0);
            await GetDb().SetAddAsync(UnionBrawlKey.SignupQueue(), unionId);
        }
        public async Task<List<longid>> SignupQueue_GetAll()
        {
            RedisValue[] redisValues = await GetDb().SetMembersAsync(UnionBrawlKey.SignupQueue());
            return redisValues.Select(value => RedisUtils.ParseLongId(value)).ToList();
        }
        // 赛季【开始】要清空 DoStartSeason
        // （世界之王是结束时清空）
        public async Task SignupQueue_Clear()
        {
            await GetDb().KeyDeleteAsync(UnionBrawlKey.SignupQueue());
        }

        //// 一个组里有哪些联盟 id，用于随机对手、清理

        // GGlobal 初始化 ✓
        public async Task GroupUnionIds_Add(int groupId, longid unionId)
        {
            await GetDb().SetAddAsync(UnionBrawlKey.GroupUnionIds(groupId), unionId);
        }
        // 开战初始化 ✓
        public async Task GroupUnionIds_Add(int groupId, List<longid> unionIds)
        {
            await GetDb().SetAddAsync(UnionBrawlKey.GroupUnionIds(groupId), unionIds.Select(x => new RedisValue(x.ToString())).ToArray());
        }
        public async Task GroupUnionIds_Clear(int groupId)
        {
            await GetDb().KeyDeleteAsync(UnionBrawlKey.GroupUnionIds(groupId));
        }
        public async Task<List<longid>> GroupUnionIds_GetAll(int groupId)
        {
            RedisValue[] redisValues = await GetDb().SetMembersAsync(UnionBrawlKey.GroupUnionIds(groupId));
            return redisValues.Select(value => RedisUtils.ParseLongId(value)).ToList();
        }

        //// 公共战报
        public async Task PublicRecords_Add(int groupId, UnionBrawlRecord record)
        {
            long count = await GetDb().ListRightPushAsync(UnionBrawlKey.PublicRecords(groupId), JsonUtils.stringify(record));
            if (count > 100)
            {
                await GetDb().ListLeftPopAsync(UnionBrawlKey.PublicRecords(groupId));
            }
        }
        public async Task<List<UnionBrawlRecord>> PublicRecords_GetAll(int groupId)
        {
            RedisValue[] redisValues = await GetDb().ListRangeAsync(UnionBrawlKey.PublicRecords(groupId), 0, -1);
            return redisValues.Select(x => JsonUtils.parse<UnionBrawlRecord>(x)).ToList();
        }
        public async Task PublicRecords_Clear(int groupId)
        {
            await GetDb().KeyDeleteAsync(UnionBrawlKey.PublicRecords(groupId));
        }

        //// 公共战报
        public async Task UnionRecords_Add(int groupId, longid unionId, UnionBrawlRecord record)
        {
            long count = await GetDb().ListRightPushAsync(UnionBrawlKey.UnionRecords(groupId, unionId), JsonUtils.stringify(record));
            if (count > 100)
            {
                await GetDb().ListLeftPopAsync(UnionBrawlKey.UnionRecords(groupId, unionId));
            }
        }
        public async Task<List<UnionBrawlRecord>> UnionRecords_GetAll(int groupId, longid unionId)
        {
            RedisValue[] redisValues = await GetDb().ListRangeAsync(UnionBrawlKey.UnionRecords(groupId, unionId), 0, -1);
            return redisValues.Select(x => JsonUtils.parse<UnionBrawlRecord>(x)).ToList();
        }
        public async Task UnionRecords_Clear(int groupId, longid unionId)
        {
            await GetDb().KeyDeleteAsync(UnionBrawlKey.UnionRecords(groupId, unionId));
        }
    }
}