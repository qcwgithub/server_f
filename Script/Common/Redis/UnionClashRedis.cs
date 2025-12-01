using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Data;
using StackExchange.Redis;
using System.Collections.Generic;
using longid = System.Int64;

namespace Script
{
    public class UnionClashRedis : WaitInitDataRedis<GroupServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }
        protected override string waitKey => GGlobalKey.UnionClashInitedFlag();

        //// playerId 对应的 unionId，开战时初始化，之后不变

        public async Task<stUnionClashPlayerUnionIdAndGroupId> PlayerToUnionId_Get(longid playerId)
        {
            RedisValue redisValue = await GetDb().HashGetAsync(UnionClashKey.PlayerToUnionId(), playerId);
            return redisValue.IsNullOrEmpty ? default : JsonUtils.parse<stUnionClashPlayerUnionIdAndGroupId>(redisValue);
        }

        // 开战初始化 ✓
        // GGlobal 初始化 ✓
        public async Task PlayerToUnionId_Add(List<longid> playerIds, stUnionClashPlayerUnionIdAndGroupId playerUnionIdAndGroupId)
        {
            await GetDb().HashSetAsync(UnionClashKey.PlayerToUnionId(), playerIds.Select(x => new HashEntry(x, JsonUtils.stringify(playerUnionIdAndGroupId))).ToArray());
        }

        public async Task PlayerToUnionId_Clear()
        {
            await GetDb().KeyDeleteAsync(UnionClashKey.PlayerToUnionId());
        }

        //// Signup Queue 报名队列

        // 正常初始化 ✓
        // GGlobal 初始化 ✓
        public async Task SignupQueue_Add(longid unionId)
        {
            MyDebug.Assert(unionId > 0);
            await GetDb().SetAddAsync(UnionClashKey.SignupQueue(), unionId);
        }
        public async Task<List<longid>> SignupQueue_GetAll()
        {
            RedisValue[] redisValues = await GetDb().SetMembersAsync(UnionClashKey.SignupQueue());
            return redisValues.Select(value => RedisUtils.ParseLongId(value)).ToList();
        }
        // 赛季【开始】要清空 DoStartSeason
        // （世界之王是结束时清空）
        public async Task SignupQueue_Clear()
        {
            await GetDb().KeyDeleteAsync(UnionClashKey.SignupQueue());
        }

        //// 一个组里有哪些联盟 id，用于随机对手、清理

        // GGlobal 初始化 ✓
        public async Task GroupUnionIds_Add(int groupId, longid unionId)
        {
            await GetDb().SetAddAsync(UnionClashKey.GroupUnionIds(groupId), unionId);
        }
        // 开战初始化 ✓
        public async Task GroupUnionIds_Add(int groupId, List<longid> unionIds)
        {
            await GetDb().SetAddAsync(UnionClashKey.GroupUnionIds(groupId), unionIds.Select(x => new RedisValue(x.ToString())).ToArray());
        }
        public async Task GroupUnionIds_Clear(int groupId)
        {
            await GetDb().KeyDeleteAsync(UnionClashKey.GroupUnionIds(groupId));
        }
        public async Task<List<longid>> GroupUnionIds_GetAll(int groupId)
        {
            RedisValue[] redisValues = await GetDb().SetMembersAsync(UnionClashKey.GroupUnionIds(groupId));
            return redisValues.Select(value => RedisUtils.ParseLongId(value)).ToList();
        }

        public async Task WaitGroupIds_Add(int groupId)
        {
            await GetDb().SetAddAsync(UnionClashKey.WaitGroupUnionIds(), groupId);
        }

        public async Task WaitGroupIds_Remove(int groupId)
        {
            await GetDb().SetRemoveAsync(UnionClashKey.WaitGroupUnionIds(), groupId);
        }

        public async Task WaitGroupIds_Clear()
        {
            await GetDb().KeyDeleteAsync(UnionClashKey.WaitGroupUnionIds());
        }

        public async Task<List<int>> WaitGroupIds_GetAll()
        {
            RedisValue[] redisValues = await GetDb().SetMembersAsync(UnionClashKey.WaitGroupUnionIds());
            return redisValues.Select(value => RedisUtils.ParseInt(value)).ToList();
        }
    }
}