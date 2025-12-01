using System.Linq;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Data;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Numerics;
using longid = System.Int64;

namespace Script
{
    public class RankingListLikeRedis : WaitInitDataRedis_ServerId<NormalServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        public override string WaitKey(int serverId) => GlobalKey.RankingListLikeInitedFlag(serverId);

        protected string Key(stTwoServerId twoServerId, RankName rankName)
        {
            RankingListConfig config = this.server.serverData.rankingListConfigLoader.GetRankingListConfig(rankName);
            return RankKey.Like(twoServerId, config);
        }

        public async Task<(RedisKey, bool)> Clear(stTwoServerId twoServerId, RankName rankName)
        {
            RedisKey key = this.Key(twoServerId, rankName);
            return (key, await this.GetDb().KeyDeleteAsync(key));
        }

        public async Task InitAdd(stTwoServerId twoServerId, List<RankingListLike> result)
        {
            var dict = result.GroupBy(x => x.rankName).ToDictionary(x => x.Key, x => x.ToArray());
            var tasks = new List<Task>();
            foreach (var kv in dict)
            {
                tasks.Add(GetDb().HashSetAsync(this.Key(twoServerId, kv.Key), kv.Value.Select(x => new HashEntry(x.memberId, x.like)).ToArray()));
            }
            await Task.WhenAll(tasks);
        }

        public async Task<int> Increase(stTwoServerId twoServerId, RankName rankName, longid memberId, int delta)
        {
            var task1 = GetDb().HashIncrementAsync(this.Key(twoServerId, rankName), memberId, delta);

            var dirtyElement = stDirtyElement.Create_RankingListLike(rankName, memberId);
            var task2 = this.server.persistence_taskQueueRedis.RPushToTaskQueue(PersistenceTaskQueueRedis.GetQueue(), dirtyElement.ToString());

            await Task.WhenAll(task1, task2);

            return (int)task1.Result;
        }

        public async Task<RankingListLike> OnlyForSave_GetFromRedis(BaseService service, RankName rankName, longid memberId)
        {
            stTwoServerId twoServerId = service.data.GetTwoServerId_byLongId(memberId);

            int like = await this.GetLike(twoServerId, rankName, memberId);
            var info = new RankingListLike();
            info.rankName = rankName;
            info.memberId = memberId;
            info.like = like;
            return info;
        }

        public async Task<int> GetLike(stTwoServerId twoServerId, RankName rankName, longid memberId)
        {
            RedisValue redisValue = await GetDb().HashGetAsync(this.Key(twoServerId, rankName), memberId);
            if (redisValue.IsNullOrEmpty)
            {
                return 0;
            }
            return RedisUtils.ParseInt(redisValue);
        }

        public async Task<List<int>> GetMany(stTwoServerId twoServerId, RankName rankName, List<longid> memberIds)
        {
            RedisValue[] redisValues = await GetDb().HashGetAsync(this.Key(twoServerId, rankName), memberIds.Select(id => new RedisValue(id.ToString())).ToArray());
            return redisValues.Select(v => RedisUtils.ParseInt(v)).ToList();
        }

        public async Task<Dictionary<longid, int>> GetManyAsDict(stTwoServerId twoServerId, RankName rankName, List<longid> memberIds)
        {
            RedisValue[] redisValues = await GetDb().HashGetAsync(this.Key(twoServerId, rankName), memberIds.Select(id => new RedisValue(id.ToString())).ToArray());
            var dict = new Dictionary<longid, int>();
            for (int i = 0; i < redisValues.Length; i++)
            {
                if (redisValues[i].IsNullOrEmpty)
                {
                    continue;
                }
                dict[memberIds[i]] = RedisUtils.ParseInt(redisValues[i]);
            }
            return dict;
        }
    }
}