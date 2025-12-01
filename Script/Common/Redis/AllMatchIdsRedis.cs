using Data;
using System.Collections.Generic;
using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Linq;
using longid = System.Int64;

namespace Script
{
    public class AllMatchIdsRedis : GWaitInitDataRedis<GroupServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        public string Key() => UnionCKey.AllMatchIds();
        protected override string waitKey => GGlobalKey.UnionCompetitionV2InitedFlag();

        public async Task Add(longid matchId)
        {
            await GetDb().SetAddAsync(Key(), matchId.ToString());
        }

        public async Task Delete(longid matchId)
        {
            await GetDb().SetRemoveAsync(Key(), matchId);
        }

        public async Task<long> Length()
        {
            return await GetDb().SetLengthAsync(Key());
        }

        public async Task<List<longid>> GetAll()
        {
            RedisValue[] redisValues = await GetDb().SetMembersAsync(Key());
            var retList = redisValues.Select(v => RedisUtils.ParseLongId(v)).ToList();
            return retList;
        }
    }
}