using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using StackExchange.Redis;

namespace Script
{
    public class DueTimeTaskQueueRedis: ServerScript<BaseServer>
    {
        Func<int, string> key_taskQueue;
        public static int[] QUEUES = new int[]
        {
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,

            99,
        };
        public static void SortQueues(List<int> queues)
        {
            queues.Sort((a, b) =>
            {
                if (a == b)
                {
                    return 0;
                }
                if (a == 99)
                {
                    return -1;
                }
                if (b == 99)
                {
                    return 1;
                }
                return a - b;
            });
        }

        public static int GetQueue()
        {
            return 99;
        }
        public static int GetQueue(int hash)
        {
            return Math.Abs(hash) % 10;
        }

        public DueTimeTaskQueueRedis(Func<int, string> key_taskQueue)
        {
            this.key_taskQueue = key_taskQueue;
        }

        IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }

        public async Task Add(int taskQueue, string member, int dueTimeS)
        {
            await this.GetDb().SortedSetAddAsync(this.key_taskQueue(taskQueue), member, dueTimeS);
        }

        public async Task AddMany(int taskQueue, Dictionary<string, int> dict)
        {
            await this.GetDb().SortedSetAddAsync(this.key_taskQueue(taskQueue), dict.Select(kv => new SortedSetEntry(kv.Key, kv.Value)).ToArray());
        }

        // dicts 
        // taskQueue -> 
        public async Task AddMany(Dictionary<int, Dictionary<string, int>> dicts)
        {
            var tasks = new List<Task>();
            foreach (var kv in dicts)
            {
                int taskQueue = kv.Key;
                tasks.Add(this.GetDb().SortedSetAddAsync(this.key_taskQueue(taskQueue), kv.Value.Select(kv => new SortedSetEntry(kv.Key, kv.Value)).ToArray()));
            }
            await Task.WhenAll(tasks);
        }

        // take = -1 表示全部
        public async Task<List<string>> GetByDueTime(int taskQueue, int dueTimeS, long take = -1)
        {
            RedisValue[] values = await this.GetDb().SortedSetRangeByScoreAsync(this.key_taskQueue(taskQueue), double.NegativeInfinity, (double)dueTimeS, Exclude.None, Order.Ascending, 0, take);
            if (values == null || values.Length == 0)
            {
                return null;
            }
            return values.Select(v => v.ToString()).ToList();
        }

        public async Task<bool> RemoveMember(int taskQueue, string member)
        {
            return await this.GetDb().SortedSetRemoveAsync(this.key_taskQueue(taskQueue), member);
        }
    }
}