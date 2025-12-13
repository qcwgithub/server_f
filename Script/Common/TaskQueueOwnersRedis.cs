using Data;
using StackExchange.Redis;

namespace Script
{
    public class TaskQueueOwnersRedis : ServerScript
    {
        string key_taskQueueOwners;

        public TaskQueueOwnersRedis(string key_taskQueueOwners)
        {
            this.key_taskQueueOwners = key_taskQueueOwners;
        }

        IDatabase GetDb()
        {
            return this.server.data.redis_db;
        }

        public async Task<Dictionary<int, TaskQueueOwner>> GetTaskQueueOwners()
        {
            HashEntry[] entries = await this.GetDb().HashGetAllAsync(this.key_taskQueueOwners);
            var dict = new Dictionary<int, TaskQueueOwner>();
            foreach (HashEntry entry in entries)
            {
                dict.Add(RedisUtils.ParseInt(entry.Name), JsonUtils.parse<TaskQueueOwner>(entry.Value));
            }
            return dict;
        }

        public async Task SetTaskQueueOwners(Dictionary<int, TaskQueueOwner> dict)
        {
            HashEntry[] entries = dict.Select(kv => new HashEntry(new RedisValue(kv.Key.ToString()), new RedisValue(JsonUtils.stringify(kv.Value)))).ToArray();
            await this.GetDb().HashSetAsync(this.key_taskQueueOwners, entries);
        }
    }
}