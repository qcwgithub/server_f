using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using Data;
using longid = System.Int64;
using System.Linq;

namespace Script
{
    public partial class TeamRedis : ServerScript<NormalServer>
    {
        // 初始
        // 创建队伍+1
        // 保存
        // 赛季结束重置
        public readonly MaxTeamId maxTeamId;

        // 初始
        // 创建队伍时+1
        // 解散队伍时-1
        // 赛季结束清空
        // 查询
        public readonly AllTeamIds allTeamIds;

        // 初始
        // 加入队伍+1
        // 离开队伍-1
        // 查询
        public readonly PlayerToTeamId playerToTeamId;

        // 初始
        // 创建队伍+1
        // 解散队伍-1
        // 查询
        public readonly TeamName teamName;

        public readonly TeamActivity activity;
        public TeamRedis(NormalServer server, TeamActivity activity)
        {
            this.Init(server);
            this.activity = activity;

            this.maxTeamId = new MaxTeamId(activity).Init(server);
            this.allTeamIds = new AllTeamIds(activity).Init(server);
            this.playerToTeamId = new PlayerToTeamId(activity).Init(server);
            this.teamName = new TeamName(activity).Init(server);
        }
    }
}