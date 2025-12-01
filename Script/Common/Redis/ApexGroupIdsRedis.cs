using Data;
using System.Collections.Generic;
using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Linq;
using longid = System.Int64;

namespace Script
{
    public class ApexGroupIdsRedis : GWaitInitDataRedis<GroupServer>
    {
        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        public string Key(int season, int grade) => ApexKey.GroupIds(season, grade);
        protected override string waitKey => GGlobalKey.ApexInitedFlag();

        public async Task Add(int season, int grade, longid groupId)
        {
            await GetDb().SetAddAsync(Key(season, grade), groupId.ToString());
        }
        public async Task AddMany(int season, int grade, longid[] groupIds)
        {
            await GetDb().SetAddAsync(Key(season, grade), groupIds.Select(id => new RedisValue(id.ToString())).ToArray());
        }

        public async Task<bool> Remove(int season, int grade, longid groupId)
        {
            bool removed = await GetDb().SetRemoveAsync(Key(season, grade), groupId);
            return removed;
        }

        public async Task<long> Length(int season, int grade)
        {
            return await GetDb().SetLengthAsync(Key(season, grade));
        }

        public async Task<List<longid>> GetAll(int season, int grade)
        {
            RedisValue[] redisValues = await GetDb().SetMembersAsync(Key(season, grade));
            var retList = redisValues.Select(v => RedisUtils.ParseLongId(v)).ToList();
            return retList;
        }
    }
}