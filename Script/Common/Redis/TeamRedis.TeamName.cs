using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using Data;
using System.Linq;

namespace Script
{
    public partial class TeamRedis
    {
        public class TeamName : ServerScript<NormalServer>
        {
            public readonly TeamActivity activity;
            public TeamName(TeamActivity activity)
            {
                this.activity = activity;
            }

            public TeamControl control => this.server.GetTeamControl(this.activity);

            IDatabase GetDb()
            {
                return this.server.baseServerData.redis_db;
            }

            string Key() => this.control.TeamKey_TeamName();

            public async Task Adds(Dictionary<long, string> dict)
            {
                HashEntry[] entries = dict.Select(kv => new HashEntry(SCUtils.Base64Encode(kv.Value), kv.Key)).ToArray();
                await GetDb().HashSetAsync(Key(), entries);
            }

            public async Task Add(string name, long teamId)
            {
                await GetDb().HashSetAsync(Key(), SCUtils.Base64Encode(name), teamId);
            }

            public async Task Delete(string name)
            {
                await GetDb().HashDeleteAsync(Key(), SCUtils.Base64Encode(name));
            }

            bool CanUse(RedisValue redisValue, long allowTeamId)
            {
                long u = RedisUtils.ParseLong(redisValue);
                return (u == 0 || u == allowTeamId);
            }

            public async Task<ECode> CheckCanUseName(string name, long allowTeamId)
            {
                bool b1 = !string.IsNullOrEmpty(name);
                if (b1)
                {
                    RedisValue redisValue = await GetDb().HashGetAsync(Key(), SCUtils.Base64Encode(name));
                    if (!this.CanUse(redisValue, allowTeamId))
                    {
                        return ECode.TeamNameExisted;
                    }
                }

                return ECode.Success;
            }

            public async Task Clear()
            {
                await GetDb().KeyDeleteAsync(Key());
            }
        }
    }
}