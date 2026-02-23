using Data;
using MessagePack;
using StackExchange.Redis;

namespace Script
{
    public class UserFriendChatInBoxRedis : ServerScript
    {
        public UserFriendChatInBoxRedis(Server server) : base(server)
        {

        }

        public IDatabase GetDb()
        {
            return this.server.data.redis_db;
        }

        string Key(long roomId)
        {
            return UserKey.FriendChatInBox(roomId);
        }

        public async Task Add(UserFriendChatInBoxItem item)
        {
            MyDebug.Assert(message.roomId > 0);
            string key = Key(message.roomId);

            byte[] bytes = MessagePackSerializer.Serialize(message);
            await Task.WhenAll(
                this.GetDb().ListRightPushAsync(key, bytes),
                this.server.persistence_taskQueueRedis.RPushToTaskQueue(0, DirtyElementManual.FriendChatMessagesEncode(message.roomId))
            );
        }

        public async Task<ChatMessage[]> GetAll(long roomId)
        {
            RedisValue[] redisValues = await this.GetDb().ListRangeAsync(Key(roomId), 0, -1);
            return redisValues.Select(x => MessagePackSerializer.Deserialize<ChatMessage>(x)).ToArray();
        }

        public async Task Trim(long roomId, int count)
        {
            await this.GetDb().ListTrimAsync(Key(roomId), 0, count - 1);
        }
    }
}