using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Data;
using StackExchange.Redis;
using System.Collections.Generic;
using longid = System.Int64;

namespace Script
{
    public class UnionCompetitionRedis : GWaitInitDataRedis<GroupServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }
        protected override string waitKey => GGlobalKey.UnionCompetitionV2InitedFlag();

        //// Wait Queue 匹配队列

        public async Task AddToWaitQueue(UnionSignupParam param)
        {
            MyDebug.Assert(param.unionId > 0);
            await GetDb().HashSetAsync(UnionCKey.WaitQueue(), param.unionId, JsonUtils.stringify(param));
        }

        public async Task<List<UnionSignupParam>> GetWaitQueue()
        {
            HashEntry[] hashEntries = await GetDb().HashGetAllAsync(UnionCKey.WaitQueue());
            return hashEntries.Select(entry => JsonUtils.parse<UnionSignupParam>(entry.Value)).ToList();
        }

        public async Task RemoveFromWaitQueue(longid unionId1, longid unionId2)
        {
            await GetDb().HashDeleteAsync(UnionCKey.WaitQueue(), new RedisValue[] { new RedisValue(unionId1.ToString()), new RedisValue(unionId2.ToString()) });
        }

        // 联盟解散时，其实也可以不用，因为匹配时也会检查
        public async Task RemoveFromWaitQueue(longid unionId)
        {
            await GetDb().HashDeleteAsync(UnionCKey.WaitQueue(), new RedisValue(unionId.ToString()));
        }

        // 赛季结束要清空
        public async Task ClearWaitQueue()
        {
            await GetDb().KeyDeleteAsync(UnionCKey.WaitQueue());
        }
    }
}