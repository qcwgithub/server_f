using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using Data;
using System;
using longid = System.Int64;

namespace Script
{
    public class MaxMailIdRedis : GMaxIdRedis
    {
        public override string Key() => GGlobalKey.MaxMailId();

        public override async Task<longid> AllocId()
        {
            Task<longid> task1 = base.AllocId();
            Task task2 = this.server.persistence_taskQueueRedis.RPushToTaskQueue(0, DirtyElementManual.MAX_MAIL_ID);
            await Task.WhenAll(task1, task2);
            return task1.Result;
        }
    }
}