using log4net;
using System;
using StackExchange.Redis;
using System.Threading.Tasks;
using Data;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using longid = System.Int64;

namespace Script
{
    public static class RedisUtils
    {
        public static Version V6_2_0 = new Version(6, 2, 0);
        // public static async void RemoveExpire(IDatabase db, string key)
        // {
        //     await db.KeyPersistAsync(key);
        // }

        // public static async void SetExpire(IDatabase db, string key, int seconds, bool onlyWhenExpireExist)
        // {
        //     Console.WriteLine("SetExpire {0}{1}", key, onlyWhenExpireExist ? " XX" : "");

        //     // 注：XX 选项是 redis version >= 7.0.0 才有的
        //     // https://redis.io/commands/expire
        //     if (onlyWhenExpireExist)
        //     {
        //         RedisResult redisResult = await db.ExecuteAsync("EXPIRE",
        //             new string[] { key, seconds.ToString(), "XX" });

        //         Console.WriteLine("redisResult " + redisResult);
        //     }
        //     else
        //     {
        //         await db.ExecuteAsync("EXPIRE",
        //             new string[] { key });
        //     }
        // }

        public static async Task<T> GetFromJson<T>(IDatabase db, RedisKey key) where T : class
        {
            RedisValue redisValue = await db.StringGetAsync(key);
            if (redisValue.IsNullOrEmpty)
            {
                return null;
            }
            return JsonUtils.parse<T>(redisValue);
        }

        public static async Task SaveAsJson<T>(IDatabase db, RedisKey key, T data, TimeSpan? expiry) where T : class
        {
            await db.StringSetAsync(key, JsonUtils.stringify(data), expiry);
        }
        public static async Task<List<T>> GetListJson<T>(IDatabase db, RedisKey key)
        {
            RedisValue[] redisValues = await db.ListRangeAsync(key);
            if (redisValues == null || redisValues.Length == 0)
            {
                return null;
            }

            var list = new List<T>();
            foreach (var v in redisValues)
            {
                if (!v.IsNullOrEmpty)
                    list.Add(JsonUtils.parse<T>(v));
            }

            return list;
        }

        public static long ParseLong(RedisValue redisValue, long defaultValue = 0)
        {
            long v;
            if (redisValue.TryParse(out v))
            {
                return v;
            }
            return defaultValue;
        }

        public static longid ParseLongId(RedisValue redisValue, longid defaultValue = 0)
        {
            if (redisValue.TryParse(out long v))
            {
                return v.i_am_sure_this_is_ok();
            }
            return defaultValue;
        }

        public static int ParseInt(RedisValue redisValue, int defaultValue = 0)
        {
            int v;
            if (redisValue.TryParse(out v))
            {
                return v;
            }
            return defaultValue;
        }

        public static Task<string> LockOnce(IDatabase db, string key, int lockTimeS, ILog logger)
        {
            return Lock(db, key, lockTimeS, 1, 0, logger);
        }

        // 锁
        // lockTimeS 锁多少秒
        // Use Case 1 tryCount == -1 锁到成功为止
        // Use Case 2 tryCount == 1 
        // Use Case 3 tryCount >= 1
        public static async Task<string> Lock(IDatabase db, string key,
            int lockTimeS,
            int tryCount,
            int tryIntervalMs,
            ILog logger)
        {
            MyDebug.Assert(tryCount != 0);
            if (tryCount < 0)
            {
                MyDebug.Assert(tryCount == -1);
            }
            else if (tryCount > 1)
            {
                MyDebug.Assert(tryIntervalMs > 0);
            }

            string lockValue = System.Guid.NewGuid().ToString();

            for (int i = 0; (tryCount == -1 || i < tryCount); i++)
            {
                if (i > 0)
                {
                    logger.Warn($"Lock '{key}' failed, will retry...{i}");
                    await Task.Delay(tryIntervalMs);
                }

                bool success = await db.StringSetAsync(new RedisKey(key), new RedisValue(lockValue),
                    TimeSpan.FromSeconds(lockTimeS), When.NotExists);

                if (success)
                {
                    return lockValue;
                }
            }

            return null;
        }

        /*
        public static async Task<string> LockMulti(IDatabase db, string[] keys,
            int lockTimeS,
            int tryCount,
            int tryIntervalMs,
            ILog logger)
        {
            if (keys.Length == 1)
            {
                return await Lock(db, keys[0], lockTimeS, tryCount, tryIntervalMs, logger);
            }

            MyDebug.Assert(tryCount != 0);
            if (tryCount < 0)
            {
                MyDebug.Assert(tryCount == -1);
            }
            else if (tryCount > 1)
            {
                MyDebug.Assert(tryIntervalMs > 0);
            }

            string lockValue = System.Guid.NewGuid().ToString();

            var pairs = keys.Select(k => new KeyValuePair<RedisKey, RedisValue>(k, lockValue)).ToArray();

            for (int i = 0; (tryCount == -1 || i < tryCount); i++)
            {
                if (i > 0)
                {
                    logger.Warn($"Lock {JsonUtils.stringify(keys)} failed, will retry...");
                    await Task.Delay(tryIntervalMs);
                }

                bool success = await db.StringSetAsync(pairs, When.NotExists);
                if (success)
                {
                    foreach (string key in keys)
                    {
                        db.KeyExpireAsync(key, TimeSpan.FromSeconds(lockTimeS)).Forget();
                    }

                    return lockValue;
                }
            }

            return null;
        }
        */

        public static async Task Unlock(IDatabase db, string key, string lockValue)
        {
            RedisValue redisValue = await db.StringGetAsync(key);
            if (redisValue == lockValue)
            {
                bool removed = await db.KeyDeleteAsync(key);
                // MyDebug.Assert(removed);
            }
        }

        public static async Task UnlockMulti(IDatabase db, string[] keys, string lockValue)
        {
            if (keys.Length == 1)
            {
                await Unlock(db, keys[0], lockValue);
                return;
            }

            RedisValue[] redisValues = await db.StringGetAsync(keys.Select(_ => new RedisKey(_)).ToArray());

            var list = new List<RedisKey>();
            for (int i = 0; i < redisValues.Length; i++)
            {
                if (redisValues[i] == lockValue)
                {
                    list.Add(keys[i]);
                }
            }

            if (list.Count > 0)
            {
                long removed = await db.KeyDeleteAsync(list.ToArray());
                MyDebug.Assert(removed == list.Count);
            }

            // foreach (string key in keys)
            // {
            //     RedisValue redisValue = await db.StringGetAsync(key);
            //     if (redisValue == lockValue)
            //     {
            //         db.KeyDeleteAsync(key).Forget();
            //     }
            // }
        }

        public static async Task<bool> IsLocked(IDatabase db, string key)
        {
            RedisValue redisValue = await db.StringGetAsync(key);
            if (redisValue.IsNullOrEmpty)
            {
                return false;
            }
            return true;
        }

        public static async Task WaitUntilUnlocked(IDatabase db, string key, int intervalMs = 5)
        {
            while (true)
            {
                RedisValue redisValue = await db.StringGetAsync(key);
                if (redisValue.IsNullOrEmpty)
                {
                    break;
                }

                await Task.Delay(intervalMs);
            }
        }
        
        public static async Task<bool> KeyCopy(BaseServer server, IDatabase db, string src, string dest)
        {
            // https://redis.io/commands/copy/
            if (server.baseServerData.redisVersion >= V6_2_0)
            {
                return await db.KeyCopyAsync(src, dest, -1, replace: true);
            }
            else
            {
                byte[] bytes = await db.KeyDumpAsync(src);
                if (bytes == null)
                {
                    return false;
                }

                await db.KeyDeleteAsync(dest);
                await db.KeyRestoreAsync(dest, bytes);
                return true;
            }
        }

        public static async Task IterateSortedSet(IDatabase db, string key, Order order, Func<int, RedisValue, Task> func)
        {
            long total = await db.SortedSetLengthAsync(key);

            long minId = 0;
            long maxId = 0;
            const int STEP = 100;
            int index = 0;
            while (true)
            {
                maxId += STEP;
                RedisValue[] values = await db.SortedSetRangeByRankAsync(key, minId, maxId, order);
                if (values.Length == 0)
                {
                    break;
                }

                foreach (RedisValue value in values)
                {
                    await func(index, value);
                    index++;
                }

                // logger.InfoFormat("{0} '{1}' [{2}, {3}] of {4}...", this.msgType, config.rankName, minId + 1, minId + entries.Length, total);

                minId = maxId + 1;
                maxId += STEP;
            }
        }

        public static async Task IterateSortedSet(IDatabase db, string key, Order order, Func<int, SortedSetEntry, Task> func)
        {
            long total = await db.SortedSetLengthAsync(key);

            long minId = 0;
            long maxId = 0;
            const int STEP = 100;
            int index = 0;
            while (true)
            {
                maxId += STEP;
                SortedSetEntry[] entries = await db.SortedSetRangeByRankWithScoresAsync(key, minId, maxId, order);
                if (entries.Length == 0)
                {
                    break;
                }

                foreach (SortedSetEntry entry in entries)
                {
                    await func(index, entry);
                    index++;
                }

                // logger.InfoFormat("{0} '{1}' [{2}, {3}] of {4}...", this.msgType, config.rankName, minId + 1, minId + entries.Length, total);

                minId = maxId + 1;
                maxId += STEP;
            }
        }
        
        public static async Task<ECode> Wait(NormalService service, string what, Func<Task<bool>> action)
        {
            while (true)
            {
                bool ok = await action();
                if (ok)
                {
                    return ECode.Success;
                }

                if (service.IsShuttingDown())
                {
                    return ECode.ServiceIsShuttingDown;
                }

                service.logger.InfoFormat("Wait '{0}'", what);

                await Task.Delay(100);
            }
        }
    }
}