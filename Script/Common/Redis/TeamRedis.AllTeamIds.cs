using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using Data;
using longid = System.Int64;
using System.Linq;

namespace Script
{
    public partial class TeamRedis
    {
        public class AllTeamIds : ServerScript<NormalServer>
        {
            public readonly TeamActivity activity;
            public AllTeamIds(TeamActivity activity)
            {
                this.activity = activity;
            }

            public TeamControl control => this.server.GetTeamControl(this.activity);

            IDatabase GetDb()
            {
                return this.server.baseServerData.redis_db;
            }

            public string Key(int groupId) => this.control.TeamKey_AllTeamIds(groupId);

            public async Task<(RedisKey, bool)> Clear(int groupId)
            {
                RedisKey key = this.Key(groupId);
                return (key, await this.GetDb().KeyDeleteAsync(key));
            }

            public async Task<bool> Exist(int groupId, long teamId)
            {
                return await this.GetDb().SetContainsAsync(Key(groupId), teamId);
            }

            public async Task Adds(int groupId, List<long> idList)
            {
                var array = idList.Select(id => new RedisValue(id.ToString())).ToArray();
                await GetDb().SetAddAsync(Key(groupId), array);
            }

            public async Task Add(int groupId, long teamId)
            {
                await GetDb().SetAddAsync(Key(groupId), teamId.ToString());
            }

            public async Task<bool> Contains(int groupId, long teamId)
            {
                return await GetDb().SetContainsAsync(Key(groupId), teamId);
            }

            public async Task Delete(int groupId, long teamId)
            {
                await GetDb().SetRemoveAsync(Key(groupId), teamId);
            }

            public async Task<List<long>> GetAll(int groupId)
            {
                RedisValue[] redisValues = await GetDb().SetMembersAsync(Key(groupId));
                var retList = redisValues.Select(v => RedisUtils.ParseLong(v)).ToList();
                return retList;
            }

            public async Task<List<long>> RandomGets(int groupId, int count)
            {
                RedisValue[] redisValues = await GetDb().SetRandomMembersAsync(Key(groupId), count);
                if (redisValues == null || redisValues.Length == 0)
                {
                    return new List<long>();
                }
                var retList = redisValues.Select(v => RedisUtils.ParseLong(v)).ToList();
                return retList;
            }
        }
    }
}