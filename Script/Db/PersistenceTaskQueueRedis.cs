using StackExchange.Redis;
using Data;

namespace Script
{
    public class PersistenceTaskQueueRedis : ServerScript
    {
        Func<int, string> key_taskQueueList;
        Func<int, string> key_taskQueueSortedSet;

        // 队列分为这几个
        // 99 表示全局的，或者不带 id 的数据
        // 有带 id 的数据，对应的队列是 id % 10
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

        public PersistenceTaskQueueRedis(Server server, Func<int, string> key_taskQueueList, Func<int, string> key_taskQueueSortedSet) : base(server)
        {
            this.key_taskQueueList = key_taskQueueList;
            this.key_taskQueueSortedSet = key_taskQueueSortedSet;
        }

        IDatabase GetDb()
        {
            return this.server.data.redis_db;
        }

        public async Task<List<stDirtyElementWithTime>> LRangeOfTaskQueue(int taskQueue, int take)
        {
            RedisValue[] values = await this.GetDb().ListRangeAsync(this.key_taskQueueList(taskQueue), 0, take - 1);
            return values.Select(v => stDirtyElementWithTime.FromString(v.ToString())).ToList();
        }

        async Task LPopFromTaskQueue(int taskQueue, int count, log4net.ILog logger)
        {
            if (this.server.data.redisVersion >= RedisUtils.V6_2_0)
            {
                RedisValue[] values = await this.GetDb().ListLeftPopAsync(this.key_taskQueueList(taskQueue), count);
                if (values.Length != count)
                {
                    logger.ErrorFormat("PersistenceTaskQueueRedis.LPopFromTaskQueue values.Length {0} error, should be {1}", values.Length, count);
                }
            }
            else
            {
                var tasks = new List<Task>();
                for (int i = 0; i < count; i++)
                {
                    tasks.Add(this.GetDb().ListLeftPopAsync(this.key_taskQueueList(taskQueue)));
                }
                await Task.WhenAll(tasks);
            }
        }

        public async Task RPushToTaskQueue(int taskQueue, string dirtyElement)
        {
            await this.GetDb().ListRightPushAsync(this.key_taskQueueList(taskQueue), stDirtyElementWithTime.sToString(dirtyElement, TimeUtils.GetTimeS()));
        }

        public async Task AddToSortedSetAndPopFromList(int taskQueue, Dictionary<string, int> de2time, int popCount, log4net.ILog logger)
        {
            // 注意是 SortedSetWhen.NotExists
            var task1 = this.GetDb().SortedSetAddAsync(this.key_taskQueueSortedSet(taskQueue), de2time.Select(kv => new SortedSetEntry(kv.Key, kv.Value)).ToArray(), SortedSetWhen.NotExists);
            var task2 = this.LPopFromTaskQueue(taskQueue, popCount, logger);
            await Task.WhenAll(task1, task2);

            // if (de2time.Count == 1 && task1.Result == 0)
            // {
            //     var ie = de2time.GetEnumerator();
            //     ie.MoveNext();
            //     logger.InfoFormat("~~~~ taskQueue {0} {1} {2}", taskQueue, ie.Current.Key, ie.Current.Value);
            // }
        }

        public async Task<List<string>> GetByDueTimeS(int taskQueue, long? dueTimeS, long take = -1)
        {
            RedisValue[] values = await this.GetDb().SortedSetRangeByScoreAsync(this.key_taskQueueSortedSet(taskQueue),
                double.NegativeInfinity, dueTimeS == null ? double.PositiveInfinity : (double)dueTimeS.Value,
                Exclude.None, Order.Ascending, 0, take);

            if (values == null || values.Length == 0)
            {
                return null;
            }
            return values.Select(v => v.ToString()).ToList();
        }

        public async Task RemoveFromSortedSet(int taskQueue, List<string> dirtyElements)
        {
            long removed = await this.GetDb().SortedSetRemoveAsync(this.key_taskQueueSortedSet(taskQueue), dirtyElements.Select(x => new RedisValue(x)).ToArray());
            MyDebug.Assert(removed == dirtyElements.Count);
        }

        public async Task<(long, long)[]> GetLengthInfo(int[] taskQueues)
        {
            var tasks = new List<Task<long>>();
            foreach (int taskQueue in taskQueues)
            {
                tasks.Add(this.GetDb().ListLengthAsync(this.key_taskQueueList(taskQueue)));
                tasks.Add(this.GetDb().SortedSetLengthAsync(this.key_taskQueueSortedSet(taskQueue)));
            }
            await Task.WhenAll(tasks);

            var ret = new (long, long)[taskQueues.Length];
            for (int i = 0; i < taskQueues.Length; i++)
            {
                ret[i] = (tasks[i * 2].Result, tasks[i * 2 + 1].Result);
            }
            return ret;
        }
    }
}