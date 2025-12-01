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
        public class MaxTeamId : MaxIdRedis_small<TeamActivity>
        {
            public TeamActivity activity => this.t;
            public MaxTeamId(TeamActivity activity) : base(activity)
            {

            }

            public TeamControl control => this.server.GetTeamControl(this.activity);

            protected override string waitKey => this.control.waitKey;
            public override string Key() => this.control.TeamKey_MaxTeamId();

            //-------------------------------------------------------------------------------------

            public async Task<bool> SetMaxId(long maxId, bool save)
            {
                var task1 = base.SetMaxId(maxId);
                var task2 = save ? this.server.persistence_taskQueueRedis.RPushToTaskQueue(0, DirtyElementManual.MaxTeamIdEncode(this.activity)) : Task.CompletedTask;
                await Task.WhenAll(task1, task2);
                return task1.Result;
            }

            public override async Task<long> AllocId()
            {
                Task<long> task1 = base.AllocId();
                Task task2 = this.server.persistence_taskQueueRedis.RPushToTaskQueue(0, DirtyElementManual.MaxTeamIdEncode(this.activity));
                await Task.WhenAll(task1, task2);
                return task1.Result;
            }
        }
    }
}