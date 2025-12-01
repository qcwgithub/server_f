using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using Data;
using longid = System.Int64;

namespace Script
{
    public class MaxPlayerIdRedis : MaxIdRedis
    {
        public override string WaitKey(int serverId) => GlobalKey.MaxPlayerIdInitedFlag(serverId);
        public override string Key(int serverId) => GlobalKey.MaxPlayerId(serverId);

        async Task<List<longid>> AllocIds(int serverId, int count)
        {
            longid last = (await GetDb().StringIncrementAsync(Key(serverId), count)).i_am_sure_this_is_ok();
            longid first = (last - count + 1).i_am_sure_this_is_ok();
            var list = new List<longid>();
            for (int i = 0; i < count; i++)
            {
                list.Add((first + i).i_am_sure_this_is_ok());
            }
            return longidext.CheckLongId(list);
        }

        public async Task<List<longid>> AllocPlayerIds(int serverId, int count)
        {
            var task1 = this.AllocIds(serverId, count);
            Task task2 = this.server.persistence_taskQueueRedis.RPushToTaskQueue(0, DirtyElementManual.MaxPlayerIdEncode(serverId));
            await Task.WhenAll(task1, task2);
            return longidext.CheckLongId(task1.Result);
        }

        public override async Task<longid> AllocId(int serverId)
        {
            Task<longid> task1 = base.AllocId(serverId);
            Task task2 = this.server.persistence_taskQueueRedis.RPushToTaskQueue(0, DirtyElementManual.MaxPlayerIdEncode(serverId));
            await Task.WhenAll(task1, task2);
            return longidext.CheckLongId(task1.Result);
        }
    }
}