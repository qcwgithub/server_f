using System;
using Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Diagnostics;
using System.Linq;

namespace Script
{
    // Cache 管 Redis 和 mongodb
    // 如果数据在 Redis 中没有，会尝试从 mongodb 中加载，并保证只加载一次
    // 数据特点：
    // 1 一个 id 一条。例如 playerId, unionId
    // 2 不是全部加载到 redis 的，只有用到的时候才会加载，超时了就从 redis 消失了
    public abstract partial class DataProxy<DataType, P1, P2> : ServerScript where DataType : class, ICanBePlaceholder
    {
        public DataProxy(Server server) : base(server)
        {
            
        }

        protected abstract IDatabase GetDb();
        protected abstract stDirtyElement DirtyElement(P1 p1, P2 p2);
        protected abstract RedisKey Key(P1 p1, P2 p2);
        protected abstract bool CanExpire();
        protected virtual string RedisValueFormat() => "json";

        // hash
        protected virtual DataType FromHashEntries(P1 p1, P2 p2, HashEntry[] entries) => throw new NotImplementedException();
        protected virtual HashEntry[] ToHashEntries(DataType data) => throw new NotImplementedException();

        // single
        protected virtual DataType FromSingleValue(P1 p1, P2 p2, RedisValue redisValue) => throw new NotImplementedException();
        protected virtual RedisValue ToSingleValue(DataType data) => throw new NotImplementedException();

        protected async Task<DataType?> GetFromRedis(P1 p1, P2 p2)
        {
            string format = this.RedisValueFormat();
            switch (format)
            {
                case "json":
                    return await RedisUtils.GetFromJson<DataType>(this.GetDb(), this.Key(p1, p2));

                case "hash":
                    {
                        HashEntry[] entries = await this.GetDb().HashGetAllAsync(this.Key(p1, p2));
                        if (entries.Length == 0)
                        {
                            return null;
                        }
                        return this.FromHashEntries(p1, p2, entries);
                    }

                case "single":
                    {
                        RedisValue redisValue = await this.GetDb().StringGetAsync(this.Key(p1, p2));
                        return this.FromSingleValue(p1, p2, redisValue);
                    }

                default:
                    throw new Exception("Not handle redis format: " + format);
            }
        }
        protected abstract DataType CreatePlaceholder(P1 p1, P2 p2);

        // 当数据不在 redis 中时，需要从数据库加载
        // 此时需要锁一下，防止多人同时加载
        protected abstract string GetLockKeyForLoadFromDBToRedis(P1 p1, P2 p2);
        protected abstract Task<(ECode, DataType)> LoadFromDB(ConnectToDbService connectToDBService, P1 p1, P2 p2);
        // protected abstract bool SaveImmediately();
        // protected abstract Task<ECode> SaveToDB(DataType info);
        protected async Task SaveToRedis(P1 p1, P2 p2, DataType data)
        {
            TimeSpan? expiry = null;
            if (this.CanExpire())
            {
                expiry = this.RandExpiry();
            }

            string format = this.RedisValueFormat();
            switch (format)
            {
                case "json":
                    await RedisUtils.SaveAsJson(this.GetDb(), this.Key(p1, p2), data, expiry);
                    break;

                case "hash":
                    {
                        HashEntry[] entries = this.ToHashEntries(data);
                        await this.GetDb().HashSetAsync(this.Key(p1, p2), entries);
                    }
                    break;

                case "single":
                    {
                        RedisValue redisValue = this.ToSingleValue(data);
                        await this.GetDb().StringSetAsync(this.Key(p1, p2), redisValue);
                    }
                    break;

                default:
                    throw new Exception("Not handle redis format: " + format);
            }

        }

        protected abstract int GetBelongTaskQueue(P1 p1, P2 p2);

        // protected abstract Task SetExpire(P1 p1, P2 p2);
        // protected abstract Task Persist(P1 p1, P2 p2);

        // protected abstract void RemoveExpire(P1 p1, P2 p2);

        // public abstract Task<DataType> GetFromRedisOrDB(P1 p1, P2 p2);
        // public abstract Task<DataType> GetFromRedis(P1 p1, P2 p2);
        // public abstract Task Save(P1 p1, P2 p2, DataType data);

        protected async Task SaveToRedis_Persist_IncreaseDirty(P1 p1, P2 p2, DataType data)
        {
            MyDebug.Assert(!data.IsPlaceholder());

            stDirtyElement dirtyElement = this.DirtyElement(p1, p2);

            throw new Exception("TODO");

            await Task.WhenAll(
                this.SaveToRedis(p1, p2, data) // ,
                                               // this.server.persistence_taskQueueRedis.RPushToTaskQueue(this.GetBelongTaskQueue(p1, p2), dirtyElement.ToString())
            );

            // 2 *PERSIST*
            // 这里不用移除超时时间，会自动移除，参考：
            // https://redis.io/commands/expire
            // The timeout will only be cleared by commands that delete or overwrite the contents of the key, including DEL, SET, GETSET and all the *STORE commands.
            // this.RemoveExpire(p1, p2);
            // 其实也不是，是在步骤 1 时指定了 expiry = null
        }

        static readonly TimeSpan S_EXPIRY = TimeSpan.FromDays(7);
        TimeSpan RandExpiry()
        {
            int seconds = this.server.data.random.Next(86400);
            return S_EXPIRY.Subtract(TimeSpan.FromSeconds(seconds));
        }

        protected async Task<DataType?> InternalGet(ConnectToDbService connectToDBService, P1 p1, P2 p2)
        {
            DataType? data = await this.GetFromRedis(p1, p2);
            if (data != null)
            {
                if (this.CanExpire())
                {
                    await GetDb().KeyExpireAsync(Key(p1, p2), this.RandExpiry());
                }
                // if (!data.IsPlaceholder())
                // {
                //     if (0 != await this.scriptEntry.dirtyRedis.GetDirtyCount(DirtyElement(p1, p2)))
                //     {
                //         await GetDb(p1, p2).KeyPersistAsync(Key(p1, p2));
                //     }
                // }

                // 参数 true，当已经有超时存在时，继续设置超时（如果一直在读就永远不会超时）
                // 如果当前并没有超时存在，表示此时数据未保存到 MongoDB，不可以继续设置超时
                // this.SetExpire(p1, p2, true);
                return data.IsPlaceholder() ? null : data;
            }

            string lockKey = this.GetLockKeyForLoadFromDBToRedis(p1, p2);
            string lockValue = await this.server.lockRedis.LockForLoadFromDBToRedis(lockKey, Data.Program.misc_logger);
            if (lockValue != null)
            {
                ECode err;
                // this.scriptEntry.firstLogger.InfoFormat("{0} load from mongodb p1 {1} p2 {2}", this.GetType().Name, p1, p2);
                (err, data) = await this.LoadFromDB(connectToDBService, p1, p2);
                if (err != ECode.Success)
                {
                    // *1
                    // 走到这里代表加载失败了，不代表数据不存在。
                    // 如果这里返回 null 会让逻辑误以为没有数据，接下去有可能生成一份新的数据覆盖掉旧的
                    // 所以这里直接抛异常，中断掉逻辑
                    throw new Exception(string.Format("{0} load from mongodb p1={1} p2={2} error={3}", this.GetType().Name, p1, p2, err));
                }

                if (data == null)
                {
                    // 如果 mongodb 里没有，我们也需要在 redis 中存点什么，下次不需要再找 mongodb 查了。否则就叫做“缓存击穿”
                    var placeholder = this.CreatePlaceholder(p1, p2);
                    await this.SaveToRedis(p1, p2, placeholder);
                }
                else
                {
                    // 成功从 mongodb 加载到数据了，缓存到 redis 去
                    await this.SaveToRedis(p1, p2, data);
                }

                await this.server.lockRedis.UnlockForLoadFromDBToRedis(lockKey, lockValue);
                return data;
            }
            else
            {
                // 其他进程正在加载此数据，这里就等着
                await RedisUtils.WaitUntilUnlocked(this.GetDb(), lockKey);
                data = await this.GetFromRedis(p1, p2);
                if (data == null)
                {
                    // 当 *1 处异常后，这里（即正在等待的其他进程）也应该抛异常，相同原因
                    throw new Exception(string.Format("{0} re-load from redis p1={1} p2={2}, data is still null", this.GetType().Name, p1, p2));
                }
                return data.IsPlaceholder() ? null : data;
            }
        }
        public async Task<List<DataType?>> GetManyHelp(ConnectToDbService connectToDBService, List<P1> idList)
        {
            RedisValue[] values = await this.GetDb().StringGetAsync(idList.Select(id => this.Key(id, default)).ToArray());

            var list = new List<DataType?>();

            List<Task<DataType?>>? tasks = null;
            List<int>? indexes = null;
            for (int i = 0; i < values.Length; i++)
            {
                list.Add(null);
                if (!values[i].IsNullOrEmpty)
                {
                    var data = JsonUtils.parse<DataType>(values[i]!);
                    if (!data.IsPlaceholder())
                    {
                        list[i] = data;
                    }
                }
                else
                {
                    if (tasks == null)
                    {
                        tasks = new List<Task<DataType?>>();
                    }

                    if (indexes == null)
                    {
                        indexes = new List<int>();
                    }

                    tasks.Add(this.InternalGet(connectToDBService, idList[i], default));
                    indexes.Add(i);
                }
            }

            if (tasks != null)
            {
                await Task.WhenAll(tasks);

                for (int i = 0; i < tasks.Count; i++)
                {
                    int index = indexes![i];
                    list[index] = tasks[i].Result;
                }
            }

            return list;
        }

        public async Task<List<DataType?>> GetMany(ConnectToDbService connectToDBService, List<P1> idList, bool fillNullIfNotExist)
        {
            var list2 = await this.GetManyHelp(connectToDBService, idList);
            if (fillNullIfNotExist)
            {
                return list2;
            }

            var list = new List<DataType?>();
            for (int i = 0; i < idList.Count; i++)
            {
                if (list2[i] != null)
                {
                    list.Add(list2[i]);
                }
            }

            return list;
        }

        public async Task<Dictionary<P1, DataType>> GetManyAsDict(ConnectToDbService connectToDBService, List<P1> idList, bool fillNullIfNotExist)
        {
            var list2 = await this.GetManyHelp(connectToDBService, idList);

            var dict = new Dictionary<P1, DataType>();
            for (int i = 0; i < idList.Count; i++)
            {
                if (list2[i] != null || fillNullIfNotExist)
                {
                    dict[idList[i]] = list2[i];
                }
            }

            return dict;
        }
    }
}