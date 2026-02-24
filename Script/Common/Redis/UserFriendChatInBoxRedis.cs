using System.Data.Common;
using Data;
using MessagePack;
using StackExchange.Redis;

namespace Script
{
    // 短时间停留 Redis
    public class UserFriendChatInBoxRedis : ServerScript
    {
        public UserFriendChatInBoxRedis(Server server) : base(server)
        {

        }

        public IDatabase GetDb()
        {
            return this.server.data.redis_db;
        }

        string Key(long userId)
        {
            return UserKey.FriendChatInBox(userId);
        }

        public async Task Add(long userId, long roomId)
        {
            await Task.WhenAll(
                // roomId -> unread
                this.GetDb().HashIncrementAsync(Key(userId), roomId),
                // TODO 能不能只是 hash++，好像不行，有个顺序问题，先改的要先保存
                this.server.persistence_taskQueueRedis.RPushToTaskQueue(0/* ! */, DirtyElementManual.UserFriendChatInBoxEncode(userId))
            );
        }

        // (roomId, unread)[]
        public async Task<(long, int)[]> GetAll(long userId)
        {
            HashEntry[] hashEntries = await this.GetDb().HashGetAllAsync(Key(userId));
            return hashEntries.Select(entry => ((long)entry.Name, (int)entry.Value)).ToArray();
        }

        public async Task Trim(long userId, (long, int)[] roomCounts)
        {
            RedisKey key = Key(userId);
            await Task.WhenAll(roomCounts.Select(x =>
            {
                (long roomId, int count) = x;
                return this.GetDb().HashDecrementAsync(key, roomId, count);
            }));
        }
    }
}