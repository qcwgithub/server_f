using System;
using System.Threading.Tasks;
using Data;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace Script
{
    public class StartedServerIdsRedis : ServerScript<NormalServer>
    {
        public IDatabase GetDb()
        {
            return this.server.serverData.redis_db;
        }

        string Key(int serverId)
        {
            return GlobalKey.ServerIdInitedFlag(serverId);
        }

        public async Task<HashSet<int>> GetServerIdsStarted(List<int> serverIds)
        {
            RedisValue[] redisValues = await this.GetDb().StringGetAsync(serverIds.Select(x => new RedisKey(this.Key(x))).ToArray());
            var set = new HashSet<int>();
            for (int i = 0; i < serverIds.Count; i++)
            {
                if (redisValues[i] == "1")
                {
                    set.Add(serverIds[i]);
                }
            }
            return set;
        }
    }
}