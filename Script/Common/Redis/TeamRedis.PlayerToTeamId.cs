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
        public class PlayerToTeamId : ServerScript<NormalServer>
        {
            public readonly TeamActivity activity;
            public PlayerToTeamId(TeamActivity activity)
            {
                this.activity = activity;
            }

            public TeamControl control => this.server.GetTeamControl(this.activity);

            IDatabase GetDb()
            {
                return this.server.baseServerData.redis_db;
            }

            string Key() => this.control.TeamKey_PlayerToTeamId();

            public async Task<longid> Get(longid playerId)
            {
                RedisValue redisValue = await GetDb().HashGetAsync(Key(), playerId);
                return RedisUtils.ParseLongId(redisValue);
            }

            // 1 创建队伍
            // 2 直接加入
            // 3 同意加入
            public async Task Add(longid playerId, long teamId)
            {
                await GetDb().HashSetAsync(Key(), playerId, teamId);
            }

            // 初始化
            public async Task Adds(List<TeamInfo> teamInfos)
            {
                var entries = new List<HashEntry>();
                foreach (TeamInfo teamInfo in teamInfos)
                {
                    foreach (longid playerId in teamInfo.playerIds)
                    {
                        entries.Add(new HashEntry(playerId, teamInfo.teamId));
                    }
                }

                await GetDb().HashSetAsync(Key(), entries.ToArray());
            }

            // 1 离开队伍
            // 2 被踢出
            public async Task Remove(longid playerId)
            {
                await GetDb().HashDeleteAsync(Key(), playerId);
            }

            public async Task Clear()
            {
                await GetDb().KeyDeleteAsync(Key());
            }
        }
    }
}