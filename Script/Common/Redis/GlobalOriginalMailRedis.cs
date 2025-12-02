using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using StackExchange.Redis;


namespace Script
{
    public class GlobalOriginalMailRedis : GWaitInitDataRedis<GroupServer>
    {
        public GlobalOriginalMailRedis()
        {
            this.receivedPlayerIds = new ReceivedPlayerIds(this);
            this.persistReceivedPlayerIds = new PersistReceivedPlayerIds(this);
            this.excludePlayerIds = new ExcludePlayerIds(this);
            this.persistExcludePlayerIds = new PersistExcludePlayerIds(this);
            this.allIds = new AllIds(this);
        }

        public override IDatabase GetDb()
        {
            return this.server.baseServerData.redis_db;
        }
        protected override string waitKey => GGlobalKey.MailInitedFlag();

        public class ReceivedPlayerIds
        {
            GlobalOriginalMailRedis parent;
            public ReceivedPlayerIds(GlobalOriginalMailRedis parent)
            {
                this.parent = parent;
            }

            IDatabase GetDb() => this.parent.GetDb();
            string Key(long mailId) => MailKey.GlobalOriginalMailReceivedPlayerIds(mailId);

            // public async Task Add(int mailId, int receivedPlayerId)
            // {
            //     await this.GetDb().SetAddAsync(this.Key(mailId), receivedPlayerId);
            // }

            public async Task AddMany(long mailId, HashSet<long> receivedPlayerIds)
            {
                await this.GetDb().HashSetAsync(this.Key(mailId), receivedPlayerIds.Select(_ => new HashEntry(_, 1)).ToArray());
            }

            // public async Task Remove(int mailId, int receivedPlayerId)
            // {
            //     await this.GetDb().SetRemoveAsync(this.Key(mailId), receivedPlayerId);
            // }

            // public async Task RemoveMany(int mailId, List<int> receivedPlayerIds)
            // {
            //     await this.GetDb().SetRemoveAsync(this.Key(mailId), receivedPlayerIds.Select(_ => new RedisValue(_.ToString())).ToArray());
            // }

            // 列表很长，不 Get
            // public async Task<HashSet<int>> Get(int mailId)
            // {
            //     RedisValue[] redisValues = await this.GetDb().SetMembersAsync(this.Key(mailId));
            //     return redisValues.Select(_ => RedisUtils.ParseInt(_)).ToHashSet();
            // }

            public async Task<bool> AddWhenNotExists(long mailId, long playerId, bool addToPersist)
            {
                bool added = await this.GetDb().HashSetAsync(this.Key(mailId), playerId, 1, When.NotExists);
                if (added && addToPersist)
                {
                    await this.parent.persistReceivedPlayerIds.Push(mailId, playerId);
                }
                return added;
            }

            public async Task<bool> IsReceived(long mailId, long playerId)
            {
                return await this.GetDb().HashExistsAsync(this.Key(mailId), playerId);
            }
        }

        public ReceivedPlayerIds receivedPlayerIds { get; private set; }


        public class ExcludePlayerIds
        {
            GlobalOriginalMailRedis parent;
            public ExcludePlayerIds(GlobalOriginalMailRedis parent)
            {
                this.parent = parent;
            }

            IDatabase GetDb() => this.parent.GetDb();
            string Key(long mailId) => MailKey.GlobalOriginalMailExcludePlayerIds(mailId);
            
            public async Task<bool> AddWhenNotExists(long mailId, long playerId, bool addToPersist)
            {
                bool added = await this.GetDb().HashSetAsync(this.Key(mailId), playerId, 1);
                if (added && addToPersist)
                {
                    await this.parent.persistExcludePlayerIds.Push(mailId, playerId);
                }
                return added;
            }

            public async Task AddMany(long mailId, HashSet<long> excludePlayerIds)
            {
                await this.GetDb().HashSetAsync(this.Key(mailId), excludePlayerIds.Select(_ => new HashEntry(_, 1)).ToArray());
            }

            public async Task<bool> Exist(long mailId, long playerId)
            {
                return await this.GetDb().HashExistsAsync(this.Key(mailId), playerId);
            }
        }

        public ExcludePlayerIds excludePlayerIds { get; private set; }

        public class PersistReceivedPlayerIds
        {
            GlobalOriginalMailRedis parent;
            public PersistReceivedPlayerIds(GlobalOriginalMailRedis parent)
            {
                this.parent = parent;
            }

            IDatabase GetDb() => this.parent.GetDb();
            string Key(long mailId) => MailKey.GlobalOriginalMailPersistReceivedPlayerIds(mailId);

            public async Task Push(long mailId, long receivedPlayerId)
            {
                var task1 = this.GetDb().ListRightPushAsync(this.Key(mailId), receivedPlayerId);
                var task2 = this.parent.server.persistence_taskQueueRedis.RPushToTaskQueue(0, DirtyElementManual.GLOBAL_MAIL_RECEIVED_PLAYER_IDS(mailId));
                await Task.WhenAll(task1, task2);
            }

            public async Task<long[]> GetAll(long mailId)
            {
                RedisValue[] values = await this.GetDb().ListRangeAsync(this.Key(mailId));
                return values.Select(_ => RedisUtils.ParseLongId(_)).ToArray();
            }

            public async Task LeftPop(long mailId, int popCount)
            {
                if (this.parent.server.baseServerData.redisVersion >= RedisUtils.V6_2_0)
                {
                    await this.GetDb().ListLeftPopAsync(this.Key(mailId), popCount);
                }
                else
                {
                    var tasks = new List<Task>();
                    for (int i = 0; i < popCount; i++)
                    {
                        tasks.Add(this.GetDb().ListLeftPopAsync(this.Key(mailId)));
                    }
                    await Task.WhenAll(tasks);
                }
            }
        }

        public PersistReceivedPlayerIds persistReceivedPlayerIds { get; private set; }


        public class PersistExcludePlayerIds
        {
            GlobalOriginalMailRedis parent;
            public PersistExcludePlayerIds(GlobalOriginalMailRedis parent)
            {
                this.parent = parent;
            }

            IDatabase GetDb() => this.parent.GetDb();
            string Key(long mailId) => MailKey.GlobalOriginalMailPersistExcludePlayerIds(mailId);

            public async Task Push(long mailId, long excludePlayerId)
            {
                var task1 = this.GetDb().ListRightPushAsync(this.Key(mailId), excludePlayerId);
                var task2 = this.parent.server.persistence_taskQueueRedis.RPushToTaskQueue(0, DirtyElementManual.GLOBAL_MAIL_EXCLUDE_PLAYER_IDS(mailId));
                await Task.WhenAll(task1, task2);
            }

            public async Task<long[]> GetAll(long mailId)
            {
                RedisValue[] values = await this.GetDb().ListRangeAsync(this.Key(mailId));
                return values.Select(_ => RedisUtils.ParseLongId(_)).ToArray();
            }

            public async Task LeftPop(long mailId, int popCount)
            {
                if (this.parent.server.baseServerData.redisVersion >= RedisUtils.V6_2_0)
                {
                    await this.GetDb().ListLeftPopAsync(this.Key(mailId), popCount);
                }
                else
                {
                    var tasks = new List<Task>();
                    for (int i = 0; i < popCount; i++)
                    {
                        tasks.Add(this.GetDb().ListLeftPopAsync(this.Key(mailId)));
                    }
                    await Task.WhenAll(tasks);
                }
            }
        }

        public PersistExcludePlayerIds persistExcludePlayerIds { get; private set; }

        public class AllIds
        {
            GlobalOriginalMailRedis parent;
            public AllIds(GlobalOriginalMailRedis parent)
            {
                this.parent = parent;
            }

            IDatabase GetDb() => this.parent.GetDb();
            string Key(int serverId) => MailKey.GlobalOriginalMailIds(serverId);

            public async Task Add(int serverId, long mailId)
            {
                await this.GetDb().SetAddAsync(this.Key(serverId), mailId);
            }

            public async Task Remove(int serverId, long mailId)
            {
                await this.GetDb().SetRemoveAsync(this.Key(serverId), mailId);
            }

            public async Task<HashSet<long>> GetAll(int serverId)
            {
                RedisValue[] redisValues = await this.GetDb().SetMembersAsync(this.Key(serverId));
                return redisValues.Select(_ => RedisUtils.ParseLongId(_)).ToHashSet();
            }

            public async Task<bool> Contains(int serverId, long mailId)
            {
                return await this.GetDb().SetContainsAsync(this.Key(serverId), mailId);
            }
        }

        public AllIds allIds { get; private set; }
    }
}